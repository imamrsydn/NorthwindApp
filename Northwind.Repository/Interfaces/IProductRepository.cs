using Northwind.DbAccess.Models;
using Northwind.ViewModel.Product;

namespace Northwind.Repository.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync(int? categoryId, int? supplierId, bool? discontinued, bool? readyToReorder);
        Task<(List<Product> Items, int TotalCount)> GetPagedAsync(ProductFilterDto filter);
        Task<List<Product>> GetAllForReorderAsync();
        Task<List<Product>> GetProductsWithUnitsOnOrderAsync();
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
        Task SaveChangesAsync();
    }
}