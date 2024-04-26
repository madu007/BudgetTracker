using Microsoft.AspNetCore.Identity;

namespace BudgetTracker.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public string FullName { get; set; }
    }
}
