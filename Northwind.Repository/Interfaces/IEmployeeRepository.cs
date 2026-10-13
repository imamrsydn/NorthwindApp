using Northwind.DbAccess.Models;
using Northwind.ViewModel.Employee;

namespace Northwind.Repository.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetAllAsync(int? employeeId, string? fullName, string? territory);
        Task<(List<Employee> Items, int TotalCount)> GetPagedAsync(EmployeeFilterDto filter);
        Task<Employee?> GetByIdAsync(int id);
        Task<Employee?> GetByIdWithTerritoriesAsync(int id);
        Task AddAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task DeleteAsync(Employee employee);
        Task AssignTerritoriesAsync(int employeeId, List<string> territoryIds);
        Task SaveChangesAsync();
    }
}