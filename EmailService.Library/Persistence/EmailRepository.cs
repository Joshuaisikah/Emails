using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmailService.Library.Model;
using EmailService.Library.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EmailService.Library.Persistence
{
    public class EmailRepository : IEmailRepository
    {
        private readonly EmailDbContext _context;

        public EmailRepository(EmailDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Email>> GetAllEmailsAsync()
        {
            return await _context.Emails.OrderByDescending(e => e.DateSent).ToListAsync();
        }


        public async Task<Email?> GetEmailByIdAsync(Guid emailId)
        {
            return await _context.Emails.FirstOrDefaultAsync(e => e.EmailId == emailId);
        }

        public async Task<bool> SaveEmailAsync(Email email)
        {
            _context.Emails.Add(email);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

