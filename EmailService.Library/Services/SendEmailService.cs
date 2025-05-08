using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using EmailService.Library.Model;
using EmailService.Library.Repositories;
using EmailService.Library.Settings;

namespace EmailService.Library.Services
{
    public class SendEmailService : ISendEmailService
    {
        private readonly Channel<Email> _channel;
        private readonly EmailConfig _config;
        private readonly IEmailRepository _emailRepository;

        public SendEmailService(Channel<Email> channel, EmailConfig config, IEmailRepository emailRepository)
        {
            _channel = channel;
            _config = config;
            _emailRepository = emailRepository;
        }

        public async Task<Email> QueueEmailAsync(string recipient, string subject, string body)
        {
            var email = Email.Create(_config.Username, recipient, subject, body);

            await _channel.Writer.WriteAsync(email); // enqueue email for background processing

            return email;
        }

        public async Task<IEnumerable<Email?>> GetAllEmailsAsync()
        {
            return await _emailRepository.GetAllEmailsAsync();
        }

        public async Task<Email?> GetEmailByIdAsync(Guid emailId)
        {
            return await _emailRepository.GetEmailByIdAsync(emailId);
        }
    }
}
