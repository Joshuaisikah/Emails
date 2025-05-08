using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmailService.Library.Model;


namespace EmailService.Library.Repositories
{
    public interface IEmailRepository
    {
        Task<bool> SaveEmailAsync(Email email);
        Task<Email?> GetEmailByIdAsync(Guid emailId);
        Task<IEnumerable<Email?>> GetAllEmailsAsync();

    }
}
