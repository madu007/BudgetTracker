using BudgetTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Domain.Data
{
    public class AuthDBContext : IdentityDbContext<ApplicationUser>
    {
        public AuthDBContext(DbContextOptions<AuthDBContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<CategoryLimit> CategoryLimits { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var UserRoleId = "a71a55d6-99d7-4123-b4e0-1218ecb90e3e";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = UserRoleId,
                    ConcurrencyStamp = UserRoleId,
                    Name = "User",
                    NormalizedName = "User".ToUpper()
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);

            builder.Entity<Transaction>()
        .Property(t => t.Amount)
        .HasColumnType("decimal(18, 2)"); // Adjust precision and scale as needed
            builder.Entity<Budget>()
        .Property(t => t.Limit)
        .HasColumnType("decimal(18, 2)");
            builder.Entity<CategoryLimit>()
        .Property(t => t.SitLimit)
        .HasColumnType("decimal(18, 2)");
        }
    }    
}
