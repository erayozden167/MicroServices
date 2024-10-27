using Microsoft.EntityFrameworkCore;
using PurchaseApi.Models.Entities;
using System.Reflection;

namespace PurchaseApi.Infrastructure.Data
{
    internal class AppDbContext : DbContext
    {
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Refundment> Refunds { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
