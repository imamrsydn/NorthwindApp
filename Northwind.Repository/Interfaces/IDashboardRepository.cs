using Northwind.DbAccess.Models;

namespace Northwind.Repository.Interfaces
{
    public interface IDashboardRepository
    {
        Task<int> GetTotalProductsAsync();
        Task<int> GetActiveProductsAsync();
        Task<int> GetTotalCustomersAsync();
        Task<int> GetTotalEmployeesAsync();
        Task<int> GetTotalOrdersAsync();
        Task<int> GetPendingOrdersAsync();
        Task<int> GetShippedOrdersAsync();
        Task<decimal> GetTotalSalesAsync();
        Task<List<Tuple<int, string, int, decimal>>> GetTopProductsAsync(int limit);
        Task<List<Tuple<string, string, int, decimal>>> GetTopCustomersAsync(int limit);
        Task<List<Order>> GetOverdueOrdersAsync();
    }
}