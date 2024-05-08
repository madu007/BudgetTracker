using BudgetTracker.Domain.Data;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Model.DB;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Domain.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<Category> AddAsync(CategoryModel category);
        Task<Category?> UpdateAsync(int id,Category category);
        Task<Category?> DeleteAsync(int id);


    }
    public class CategoryService : ICategoryService
    {
        private readonly BudgetTrackerDbContext _budgetTrackerDb;

        public CategoryService(BudgetTrackerDbContext budgetTrackerDb)
        {
            _budgetTrackerDb = budgetTrackerDb;
        }

        public async Task<Category> AddAsync(CategoryModel category)
        {
            var add = new Category
            {
                Name = category.Name,
                Icon = category.Icon,
                Type = category.Type,
            };
            await _budgetTrackerDb.AddAsync(add);
            await _budgetTrackerDb.SaveChangesAsync();
            return add;
        }

        public async Task<Category?> DeleteAsync(int id)
        {
            var category = await _budgetTrackerDb.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category != null)
            {
                _budgetTrackerDb.Categories.Remove(category);
                await _budgetTrackerDb.SaveChangesAsync();
                return category;
            }
            return null;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _budgetTrackerDb.Categories.ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _budgetTrackerDb.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category?> UpdateAsync(int id, Category category)
        {
            var existting = await _budgetTrackerDb.Categories.FindAsync(category.Id);
            if (existting != null)
            {
                existting.Name = category.Name;
                existting.Icon = category.Icon;
                existting.Type = category.Type;

                await _budgetTrackerDb.SaveChangesAsync();
                return existting;
            }
            return null;
        }
    }
}
