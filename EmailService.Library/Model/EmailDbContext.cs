
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EmailService.Library.Model
{
    public class EmailDbContext : DbContext
    {


        public EmailDbContext(DbContextOptions<EmailDbContext> options) : base(options)
        {

        }
        public DbSet<Email> Emails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Email>().HasKey(e => e.EmailId);
        }

    }
}
