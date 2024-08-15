using Microsoft.EntityFrameworkCore;
using DuitkuMvpNetApp.Models;

namespace DuitkuMvpNetApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Transaction>()
                .HasMany(t => t.Items)
                .WithOne(i => i.Transaction)
                .HasForeignKey(i => i.MsItemsTransactionId);
        }
    }
}