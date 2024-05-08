using Microsoft.AspNetCore.Mvc.Rendering;

namespace BudgetTracker.Domain.Model.DB
{
    public class BudgetModel
    {
        public int BudgetId { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public decimal Limit { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<SelectListItem> Items { get; set; }
        // Other properties like notifications

    }
}
