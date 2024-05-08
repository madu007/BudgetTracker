using AutoMapper;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Model.DB;

namespace BudgetTracker.API
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Category, CategoryModel>().ReverseMap();
            CreateMap<CategoryModel, Category>().ReverseMap();
            CreateMap<TransactionModel, Transaction>().ReverseMap();
            CreateMap<Transaction, TransactionModel>().ReverseMap();
            CreateMap<Budget, BudgetModel>().ReverseMap();
            CreateMap<BudgetModel, Budget>().ReverseMap();
        }
    }
}
