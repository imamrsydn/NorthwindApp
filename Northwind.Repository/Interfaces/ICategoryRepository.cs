using Northwind.DbAccess.Models;
using Northwind.ViewModel.Category;

namespace Northwind.Repository.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync(string? nameFilter);
        Task<(List<Category> Items, int TotalCount)> GetPagedAsync(CategoryFilterDto filter);
        Task<Category?> GetByIdAsync(int id);
        Task<Category?> GetByIdWithProductsAsync(int id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);
        Task<bool> ExistsAsync(int id);
        Task SaveChangesAsync();
        Task<List<Tuple<int, string, int, decimal>>> GetTopProductsAsync(int limit, int categoryId);
    }
}