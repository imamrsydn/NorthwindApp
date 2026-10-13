using Northwind.DbAccess.Models;
using Northwind.ViewModel.Supplier;

namespace Northwind.Repository.Interfaces
{
    public interface ISupplierRepository
    {
        Task<List<Supplier>> GetAllAsync(string? name, string? city, string? country);
        Task<(List<Supplier> Items, int TotalCount)> GetPagedAsync(SupplierFilterDto filter);
        Task<Supplier?> GetByIdAsync(int id);
        Task<Supplier?> GetByIdWithProductsAsync(int id);
        Task AddAsync(Supplier supplier);
        Task UpdateAsync(Supplier supplier);
        Task DeleteAsync(Supplier supplier);
        Task SaveChangesAsync();
    }
}