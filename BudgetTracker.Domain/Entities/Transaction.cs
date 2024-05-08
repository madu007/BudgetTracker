using System.ComponentModel.DataAnnotations;

namespace BudgetTracker.Domain.Entities
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string? Description { get; set; }
        public virtual Category Category { get; set; }
    }
}
