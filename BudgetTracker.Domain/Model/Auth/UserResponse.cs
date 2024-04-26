using BudgetTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BudgetTracker.Domain.Model.Auth
{
    public class UserResponse
    {
        public string Token { get; set; } = null;
        public IdentityUser User { get; set; } = null;
    }
}
