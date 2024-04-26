using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Domain.Model.Auth
{
    public class TokenType
    {
        public string? Token { get; set; }
        public DateTime? ExpiryTokenDate { get; set; }
    }
}
