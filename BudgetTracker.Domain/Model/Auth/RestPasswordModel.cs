using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Domain.Model.Auth
{
    public class RestPasswordModel
    {
        [Required]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "The Password and Confirm password do not match!")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
