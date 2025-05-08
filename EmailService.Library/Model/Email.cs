
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService.Library.Model
{
    public class Email
    {
        public Guid EmailId { get; private set; }
        public string Sender { get; private set; }
        public string Recipient { get; private set; }
        public string Subject { get; private set; }
        public string MessageBody { get; private set; }
        public string Status { get; private set; }
        public DateTime? DateSent { get; private set; }
        public int NumberOfAttempts { get; private set; }

        private Email(Guid emailId, string sender, string recipient, string subject, string messageBody, string status, int numberOfAttempts)
        {
            EmailId = emailId;
            Sender = sender;
            Recipient = recipient;
            Subject = subject;
            MessageBody = messageBody;
            Status = status;
            NumberOfAttempts = numberOfAttempts;
            DateSent = null;
        }

        public static Email Create(string sender, string recipient, string subject, string messageBody)
        {
            return new Email(Guid.NewGuid(), sender, recipient, subject, messageBody, "Queued", 0);
        }

        public void MarkAsSent()
        {
            Status = "Sent";
            DateSent = DateTime.UtcNow;
        }

        public void IncrementAttempts()
        {
            NumberOfAttempts++;
        }

        public void MarkAsFailed()
        {
            Status = "Failed";
            DateSent = DateTime.UtcNow;
        }
    }
}
