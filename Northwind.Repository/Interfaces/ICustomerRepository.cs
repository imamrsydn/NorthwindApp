using Northwind.DbAccess.Models;
using Northwind.ViewModel.Customer;

namespace Northwind.Repository.Interfaces
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllAsync(string? companyName, string? contactPerson, string? region);
        Task<(List<Customer> Items, int TotalCount)> GetPagedAsync(CustomerFilterDto filter);
        Task<Customer?> GetByIdAsync(string customerId);
        Task<Customer?> GetByIdWithOrdersAsync(string customerId);
        Task<List<string>> GetTop10CustomerIdsInLast3MonthsAsync();
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(Customer customer);
        Task SaveChangesAsync();
    }
}