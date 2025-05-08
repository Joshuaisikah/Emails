using EmailService.Frontend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmailService.Frontend.Controllers
{
    public class EmailsController : Controller
    {
        private readonly EmailApiService _emailApiService;
        private readonly ILogger<EmailsController> _logger;

        public EmailsController(EmailApiService emailApiService, ILogger<EmailsController> logger)
        {
            _emailApiService = emailApiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Fetching emails for Index view");
            var emails = await _emailApiService.GetEmailsAsync();
            _logger.LogInformation("Fetched {Count} emails for Index view", emails.Count);
            return View(emails);
        }

        [HttpPost]
        public async Task<IActionResult> SendSingleEmail(string recipient, string subject, string messageBody)
        {
            _logger.LogInformation("Sending single email to {Recipient}", recipient);
            var (success, message) = await _emailApiService.SendSingleEmailAsync(recipient, subject, messageBody);
            if (success)
            {
                ViewData["SingleEmailSuccess"] = message;
                _logger.LogInformation("Single email sent successfully: {Message}", message);
            }
            else
            {
                ViewData["SingleEmailError"] = message;
                _logger.LogError("Failed to send single email: {Message}", message);
            }

            var emails = await _emailApiService.GetEmailsAsync();
            return View("Index", emails);
        }

        [HttpPost]
        public async Task<IActionResult> SendBulkEmail(string recipients, string subject, string messageBody)
        {
            try
            {
                var recipientList = recipients.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                             .Select(r => r.Trim())
                                             .ToArray();
                _logger.LogInformation("Sending bulk email to {Count} recipients", recipientList.Length);
                var (success, response) = await _emailApiService.SendBulkEmailAsync(recipientList, subject, messageBody);
                if (success)
                {
                    var message = $"Bulk email processed: {response.SuccessfullyQueued} of {response.TotalRecipients} emails queued successfully.";
                    if (response.FailedRecipients.Any())
                    {
                        message += $" Failed recipients: {string.Join(", ", response.FailedRecipients)}.";
                    }
                    ViewData["BulkEmailSuccess"] = message;
                    _logger.LogInformation("Bulk email sent successfully: {Message}", message);
                }
                else
                {
                    ViewData["BulkEmailError"] = "Failed to process bulk email.";
                    _logger.LogError("Failed to process bulk email");
                }
            }
            catch (Exception ex)
            {
                ViewData["BulkEmailError"] = $"Failed to send bulk email: {ex.Message}";
                _logger.LogError(ex, "Exception while sending bulk email");
            }

            var emails = await _emailApiService.GetEmailsAsync();
            return View("Index", emails);
        }
    }
}