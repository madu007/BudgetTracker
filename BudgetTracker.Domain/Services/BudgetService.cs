using BudgetTracker.Domain.Data;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Model.DB;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Domain.Services
{
    public interface IBudgetService
    {
        Task<List<Budget>> GetAllBudgetsAsync();
        Task<Budget> GetBudgetByIdAsync(int budgetId);
        Task<Budget> AddBudgetAsync(BudgetModel budgetModel);
        Task<Budget> UpdateBudgetAsync(int BudgetId, Budget budget);
        Task<Budget> DeleteBudgetAsync(int budgetId);
    }
    public class BudgetService : IBudgetService
    {
        private readonly BudgetTrackerDbContext _dbContext;

        public BudgetService(BudgetTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Budget> AddBudgetAsync(BudgetModel budgetModel)
        {
            var budget = new Budget
            {
                Limit = budgetModel.Limit,
                BudgetId = budgetModel.BudgetId,
                CategoryId = budgetModel.CategoryId,
                EndDate = budgetModel.EndDate,
                StartDate = budgetModel.StartDate,
                UserId = budgetModel.UserId,
            };
            var sitLimit = GetSitLimitForCategory(budget.CategoryId);
            if (budget.Limit >  sitLimit)
            {

            }
            _dbContext.Budgets.Add(budget);
            await _dbContext.SaveChangesAsync();
            return budget;
        }

        public async Task<Budget> DeleteBudgetAsync(int budgetId)
        {
            var budget = await _dbContext.Budgets.FindAsync(budgetId);
            if (budget != null)
            {
                _dbContext.Budgets.Remove(budget);
                await _dbContext.SaveChangesAsync();
            }
            return null;
        }

        public async Task<List<Budget>> GetAllBudgetsAsync()
        {
            return await _dbContext.Budgets.ToListAsync();
        }

        public async Task<Budget> GetBudgetByIdAsync(int budgetId)
        {
            return await _dbContext.Budgets.Include(b => b.Category).FirstOrDefaultAsync(b => b.BudgetId == budgetId);
        }

        public async Task<Budget> UpdateBudgetAsync(int BudgetId, Budget budget)
        {
            var existBudget = await _dbContext.Budgets.FindAsync(budget.BudgetId);
            if (existBudget != null)
            {
                existBudget.StartDate = budget.StartDate;
                existBudget.EndDate = budget.EndDate;
                existBudget.Limit = budget.Limit;   
                existBudget.UserId = budget.UserId;
                existBudget.Category = budget.Category;
            }
            return null;
        }

        private decimal GetSitLimitForCategory(int categoryId)
        {
            var categoryLimit = _dbContext.CategoryLimits
        .FirstOrDefault(cl => cl.CategoryId == categoryId);

            if (categoryLimit != null)
            {
                return (decimal)categoryLimit.SitLimit;
            }
            else
            {
                return decimal.MaxValue;
            }
            
        }
    }
}
