using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Domain.Model.DB
{
    public class CategoryModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Icon { get; set; }
        [Required]
        public string Type { get; set; } = "Expense";
    }
}
