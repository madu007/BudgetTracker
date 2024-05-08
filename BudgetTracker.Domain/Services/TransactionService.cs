using BudgetTracker.Domain.Data;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Model.DB;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Domain.Services
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GatAllTransactionAsync();
        Task<Transaction> GetTransactionByIdAsync(int  transactionId);
        Task<Transaction> AddTransactionAsync(TransactionModel transaction);
        Task<Transaction> UpdateTransactionAsync(int transactionId,Transaction transaction);
        Task<Transaction> DeleteTransactionAsync(int transactionId);
    }
    public class TransactionService : ITransactionService
    {
        private readonly BudgetTrackerDbContext _context;

        public TransactionService(BudgetTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction> AddTransactionAsync(TransactionModel transactionModel)
        {
            var transactionEntity = new Transaction
            {
                
                Amount = transactionModel.Amount,
                Date = transactionModel.Date,
                Description = transactionModel.Description,
                Category = await _context.Categories.FindAsync(transactionModel.SelectCategory)
            };
            _context.Transactions.Add(transactionEntity);
            await _context.SaveChangesAsync();
            return transactionEntity;
        }

        public async Task<Transaction> DeleteTransactionAsync(int transactionId)
        {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(x => x.TransactionId == transactionId);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
            return null;
        }

        public async Task<List<Transaction>> GatAllTransactionAsync()
        {
            return await _context.Transactions.ToListAsync();
        }

        public async Task<Transaction> GetTransactionByIdAsync(int transactionId)
        {
            return await _context.Transactions.Include(t => t.Category).FirstOrDefaultAsync(x => x.TransactionId == transactionId);
        }

        public async Task<Transaction> UpdateTransactionAsync(int transactionId, Transaction transaction)
        {
            var existingTransaction = await _context.Transactions.FindAsync(transaction.TransactionId);
            if (existingTransaction != null)
            {
                existingTransaction.Category = transaction.Category;
                existingTransaction.Amount = transaction.Amount;
                existingTransaction.Description = transaction.Description;
                existingTransaction.Date = transaction.Date;

                await _context.SaveChangesAsync();
                return existingTransaction;
            }
            return null;
        }
    }
}
