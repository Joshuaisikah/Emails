using EmailService.Library.Model;
using EmailService.Library.Repositories;
using EmailService.Library.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;
using Polly;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace EmailService.Library.Services
{
    public class EmailBackgroundService : BackgroundService
    {
        private readonly Channel<Email> _channel;
        private readonly EmailConfig _config;
        private readonly ILogger<EmailBackgroundService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public EmailBackgroundService(
            Channel<Email> channel,
            EmailConfig config,
            ILogger<EmailBackgroundService> logger,
            IServiceScopeFactory scopeFactory)
        {
            _channel = channel;
            _config = config;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Email background service started.");

            await foreach (var email in _channel.Reader.ReadAllAsync(stoppingToken))
            {
                using var scope = _scopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IEmailRepository>();

                try
                {
                    await SendEmailWithRetryAsync(email, repository, stoppingToken);
                }
                catch (Exception ex)
                {
                    email.MarkAsFailed();
                    await repository.SaveEmailAsync(email);
                    _logger.LogError(ex, $"Failed to send email to {email.Recipient} after retries.");
                }
            }
        }

        private async Task SendEmailWithRetryAsync(Email email, IEmailRepository repository, CancellationToken cancellationToken)
        {
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: _ => TimeSpan.FromSeconds(30),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        email.IncrementAttempts();
                        _logger.LogWarning(exception, $"Retry {retryCount} for email to {email.Recipient}. Waiting {timeSpan.TotalSeconds}s.");
                    });

            await retryPolicy.ExecuteAsync(async () =>
            {
                var message = new MimeMessage();
                message.Sender = MailboxAddress.Parse(_config.Username);
                message.To.Add(MailboxAddress.Parse(email.Recipient));
                message.Subject = email.Subject;

                var builder = new BodyBuilder { HtmlBody = email.MessageBody };
                message.Body = builder.ToMessageBody();

                using var smtpClient = new SmtpClient();
                await smtpClient.ConnectAsync(_config.SmtpServer, _config.Port, SecureSocketOptions.StartTls, cancellationToken);
                await smtpClient.AuthenticateAsync(_config.Username, _config.Password, cancellationToken);
                await smtpClient.SendAsync(message, cancellationToken);
                await smtpClient.DisconnectAsync(true, cancellationToken);

                email.MarkAsSent();
                await repository.SaveEmailAsync(email);
                _logger.LogInformation($"Email sent to {email.Recipient}");
            });
        }
    }
}
