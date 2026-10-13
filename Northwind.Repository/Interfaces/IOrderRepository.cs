using Northwind.DbAccess.Models;
using Northwind.ViewModel.Order;

namespace Northwind.Repository.Interfaces
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllAsync(string? customerId, int? supplierId, int? employeeId, DateTime? startDate, DateTime? endDate);
        Task<(List<Order> Items, int TotalCount)> GetPagedAsync(OrderFilterDto filter);
        Task<Order?> GetByIdAsync(int id);
        Task<Order?> GetByIdWithDetailsAsync(int id);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(Order order);
        Task SaveChangesAsync();
    }
}