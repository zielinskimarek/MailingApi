using Microsoft.EntityFrameworkCore;

namespace MailingApi.Domain.Models
{
    public class DomainContext : DbContext
    {
        public DomainContext(DbContextOptions<DomainContext> options) : base(options) { }

        public DbSet<Email> Emails { get; private set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Email>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
