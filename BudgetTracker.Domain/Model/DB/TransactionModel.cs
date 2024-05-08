using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Domain.Model.DB
{
    public class TransactionModel
    {
        
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public DateTime Date { get; set; } = DateTime.Now;
        public IEnumerable<SelectListItem> Categories { get; set; }
        [Required]
        public  int SelectCategory { get; set; }
        [Required]
        public string? Description { get; set; }

    }
}
