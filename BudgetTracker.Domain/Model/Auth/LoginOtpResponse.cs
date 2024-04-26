using BudgetTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BudgetTracker.Domain.Model.Auth
{
    public class LoginOtpResponse
    {
        public string Token { get; set; } = null;
        public bool IsTwoFactorEnable { get; set; }
        public ApplicationUser User { get; set; } = null;
    }
}
