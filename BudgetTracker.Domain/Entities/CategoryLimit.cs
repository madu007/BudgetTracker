using System.ComponentModel.DataAnnotations;


namespace BudgetTracker.Domain.Entities
{
    public class CategoryLimit
    {
        [Key]
        public int CategoryId { get; set; }
        public decimal? SitLimit { get; set; }
        public virtual Category Category { get; set; }
    }
}
