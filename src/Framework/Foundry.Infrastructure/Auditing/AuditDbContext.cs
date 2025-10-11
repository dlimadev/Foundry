using Foundry.Domain.Model.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Foundry.Infrastructure.Auditing
{
    public class AuditDbContext : DbContext
    {
        public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options) { }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // This line will now compile successfully
            modelBuilder.Entity<AuditLog>().ToTable("AuditLogs", "auditing");

            modelBuilder.Entity<AuditLog>().HasKey(a => a.Id);
            base.OnModelCreating(modelBuilder);
        }
    }
}