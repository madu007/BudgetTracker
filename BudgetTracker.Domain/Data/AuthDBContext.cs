using BudgetTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Domain.Data
{
    public class AuthDBContext : IdentityDbContext<ApplicationUser>
    {
        public AuthDBContext(DbContextOptions<AuthDBContext> options) : base(options)
        {
        }


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
        }
    }    
}
