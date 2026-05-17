using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.Models;

namespace TechMoveGLMS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Client)
                .WithMany(c => c.Contracts)
                .HasForeignKey(c => c.ClientId);

            modelBuilder.Entity<ServiceRequest>()
                .HasOne(s => s.Contract)
                .WithMany(c => c.ServiceRequests)
                .HasForeignKey(s => s.ContractId);
        }
    }
}