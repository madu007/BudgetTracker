using BudgetTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Domain.Data
{
    public class BudgetTrackerDbContext : DbContext
    {
        public BudgetTrackerDbContext(DbContextOptions<BudgetTrackerDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<CategoryLimit> CategoryLimits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaction>()
        .Property(t => t.Amount)
        .HasColumnType("decimal(18, 2)"); // Adjust precision and scale as needed
            modelBuilder.Entity<Budget>()
        .Property(t => t.Limit)
        .HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<CategoryLimit>()
        .Property(t => t.SitLimit)
        .HasColumnType("decimal(18, 2)");
        }
    }
}
