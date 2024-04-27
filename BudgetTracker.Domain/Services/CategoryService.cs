using BudgetTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Domain.Services
{
    public interface ICategoryService
    {
        Task<Category> GetAllAsync();


    }
    public class CategoryService
    {
    }
}
