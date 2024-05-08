

using System.ComponentModel.DataAnnotations;

namespace BudgetTracker.Domain.Entities
{
    public class Budget
    {
        [Key]
        public int BudgetId { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public decimal Limit { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; }
        public virtual Category Category { get; set; }
        // Other properties like notifications

    }
}
