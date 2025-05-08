using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmailService.Library.Model;


namespace EmailService.Library.Services
{
    public interface ISendEmailService
    {
        Task<Email> QueueEmailAsync(string recipient, string subject, string body);

        Task<Email?> GetEmailByIdAsync(Guid emailId);
        Task<IEnumerable<Email?>> GetAllEmailsAsync();
    }
}


