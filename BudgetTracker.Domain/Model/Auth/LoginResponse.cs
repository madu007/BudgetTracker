

namespace BudgetTracker.Domain.Model.Auth
{
    public class LoginResponse
    {
        public TokenType AccessToken { get; set; }
        public TokenType RefreshToken { get; set; }
    }
}
