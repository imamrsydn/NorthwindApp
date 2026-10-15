using Microsoft.EntityFrameworkCore;
using Northwind.DbAccess.Models;
using Northwind.Repository.Interfaces;
using Northwind.ViewModel.Employee;

namespace Northwind.Repository.Implementations
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly Northwind3Context _context;

        public EmployeeRepository(Northwind3Context context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetAllAsync(int? employeeId, string? fullName, string? territory)
        {
            var query = _context.Employees
                .Include(e => e.ReportsToNavigation)
                .Include(e => e.Territories)
                .AsQueryable();

            if (employeeId.HasValue)
                query = query.Where(e => e.EmployeeId == employeeId.Value);

            if (!string.IsNullOrWhiteSpace(fullName))
            {
                var search = fullName.Trim().ToLower();
                query = query.Where(e => (e.FirstName + " " + e.LastName).ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(territory))
            {
                query = query.Where(e => e.Territories.Any(t => t.TerritoryDescription.Contains(territory) || t.TerritoryId.Contains(territory)));
            }

            return await query.ToListAsync();
        }

        public async Task<(List<Employee> Items, int TotalCount)> GetPagedAsync(EmployeeFilterDto filter)
        {
            var query = _context.Employees
                .Include(e => e.ReportsToNavigation)
                .Include(e => e.Territories)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchKeyword))
            {
                var keyword = filter.SearchKeyword.Trim().ToLower();
                query = query.Where(e => e.FirstName.ToLower().Contains(keyword) ||
                                         e.LastName.ToLower().Contains(keyword) ||
                                         (e.Title != null && e.Title.ToLower().Contains(keyword)));
            }

            if (!string.IsNullOrWhiteSpace(filter.FirstName))
                query = query.Where(e => e.FirstName.Contains(filter.FirstName));

            if (!string.IsNullOrWhiteSpace(filter.LastName))
                query = query.Where(e => e.LastName.Contains(filter.LastName));

            if (!string.IsNullOrWhiteSpace(filter.Title))
                query = query.Where(e => e.Title != null && e.Title.Contains(filter.Title));

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(e => e.EmployeeId)
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.ReportsToNavigation)
                .Include(e => e.Territories)
                    .ThenInclude(e => e.Region)
                .Include(e => e.InverseReportsToNavigation)
                .Include(e => e.Orders)
                .Include(e => e.UserAccounts)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
        }

        public async Task<Employee?> GetByIdWithTerritoriesAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.ReportsToNavigation)
                .Include(e => e.Territories)
                    .ThenInclude(t => t.Region)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
        }

        public async Task AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
        }

        public async Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Employee employee)
        {
            _context.Employees.Remove(employee);
            await Task.CompletedTask;
        }

        public async Task AssignTerritoriesAsync(int employeeId, List<string> territoryIds)
        {
            var employee = await _context.Employees
                .Include(e => e.Territories)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (employee == null) return;

            // Reset territory lama & ganti dengan list territory baru
            employee.Territories.Clear();

            var selectedTerritories = await _context.Territories
                .Where(t => territoryIds.Contains(t.TerritoryId))
                .ToListAsync();

            foreach (var t in selectedTerritories)
            {
                employee.Territories.Add(t);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}