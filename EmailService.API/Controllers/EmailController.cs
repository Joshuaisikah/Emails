
using System.Threading.Channels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using Org.BouncyCastle.Cms;
using System.Data;
using EmailService.Library.Services;
using EmailService.API.DTOs;
using EmailService.Library.Model;

namespace EmailService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly Channel<Email> _channel;

        private readonly ISendEmailService _emailService;

        public EmailController(Channel<Email> channel, ISendEmailService emailService)
        {
            _channel = channel;

            _emailService = emailService;
        }

        [HttpPost("send-single-email")]
        public async Task<IActionResult> SendSingleEmailAsync([FromBody] EmailRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid email request.");

            var email = await _emailService.QueueEmailAsync(
                request.Recipient,
                request.Subject,
                request.MessageBody
            );

            return email?.Status switch
            {
                "Sent" => Ok(new { Message = "Email sent successfully." }),
                "Queued" => Accepted(new { Message = "Email queued for sending." }),
                "Failed" => StatusCode(500, new { Message = "Email failed to send." }),
                _ => StatusCode(500, new { Message = "Unknown email status." })
            };
        }

        [HttpPost("send-bulk-emails")]
        public async Task<IActionResult> SendBulkEmail([FromBody] BulkEmailRequestDto request)
        {
            if (!ModelState.IsValid || request?.Recipients?.Any() != true)
                return BadRequest("Invalid email request or empty recipient list.");

            var queuedEmails = new List<Email>();
            var failedRecipients = new List<string>();

            foreach (var recipient in request.Recipients)
            {
                try
                {
                    var email = await _emailService.QueueEmailAsync(
                        recipient,
                        request.Subject,
                        request.MessageBody
                    );
                    queuedEmails.Add(email);
                }
                catch (Exception ex)
                {
                    failedRecipients.Add(recipient);
                }
            }

            return Ok(new BulkEmailResponseDto
            {
                TotalRecipients = request.Recipients.Count,
                SuccessfullyQueued = queuedEmails.Count,
                FailedRecipients = failedRecipients,

            });
        }


        [HttpGet("{emailId}")]

        public async Task<IActionResult> GetEmailById(Guid id)
        {
            var email = await _emailService.GetEmailByIdAsync(id);
            if (email == null)
                return NotFound(new { Message = $"Email with ID {id} not found." });

            return Ok(email);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllEmails()
        {
            var emails = await _emailService.GetAllEmailsAsync();

            return Ok(emails);
        }
    }

}
