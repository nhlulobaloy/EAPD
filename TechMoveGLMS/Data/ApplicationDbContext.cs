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
                .HasOne(contract => contract.Client)
                .WithMany(client => client.Contracts)
                .HasForeignKey(contract => contract.ClientId);

            modelBuilder.Entity<ServiceRequest>()
                .HasOne(request => request.Contract)
                .WithMany(contract => contract.ServiceRequests)
                .HasForeignKey(request => request.ContractId);
        }
    }
}
