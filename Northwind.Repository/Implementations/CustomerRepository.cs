using Microsoft.EntityFrameworkCore;
using Northwind.DbAccess.Models;
using Northwind.Repository.Interfaces;
using Northwind.ViewModel.Customer;

namespace Northwind.Repository.Implementations
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly Northwind3Context _context;

        public CustomerRepository(Northwind3Context context)
        {
            _context = context;
        }

        public async Task<List<Customer>> GetAllAsync(string? companyName, string? contactPerson, string? region)
        {
            var query = _context.Customers
                .Include(c => c.Orders)
                    .ThenInclude(o => o.OrderDetails)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(companyName))
                query = query.Where(c => c.CompanyName.Contains(companyName));

            if (!string.IsNullOrWhiteSpace(contactPerson))
                query = query.Where(c => c.ContactName != null && c.ContactName.Contains(contactPerson));

            if (!string.IsNullOrWhiteSpace(region))
                query = query.Where(c => c.Region != null && c.Region.Contains(region));

            return await query.ToListAsync();
        }

        public async Task<(List<Customer> Items, int TotalCount)> GetPagedAsync(CustomerFilterDto filter)
        {
            var query = _context.Customers.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchKeyword))
            {
                var keyword = filter.SearchKeyword.Trim().ToLower();
                query = query.Where(c => c.CustomerId.ToLower().Contains(keyword) ||
                                         c.CompanyName.ToLower().Contains(keyword) ||
                                         (c.ContactName != null && c.ContactName.ToLower().Contains(keyword)));
            }

            if (!string.IsNullOrWhiteSpace(filter.CompanyName))
                query = query.Where(c => c.CompanyName.Contains(filter.CompanyName));

            if (!string.IsNullOrWhiteSpace(filter.ContactName))
                query = query.Where(c => c.ContactName != null && c.ContactName.Contains(filter.ContactName));

            if (!string.IsNullOrWhiteSpace(filter.Country))
                query = query.Where(c => c.Country != null && c.Country.Contains(filter.Country));

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.CustomerId)
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Customer?> GetByIdAsync(string customerId)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<Customer?> GetByIdWithOrdersAsync(string customerId)
        {
            return await _context.Customers
                .Include(c => c.Orders)
                    .ThenInclude(o => o.OrderDetails)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<List<string>> GetTop10CustomerIdsInLast3MonthsAsync()
        {
            var threeMonthsAgo = DateTime.UtcNow.AddMonths(-3);

            return await _context.Orders
                .Where(o => o.OrderDate >= threeMonthsAgo)
                .SelectMany(o => o.OrderDetails, (o, od) => new { o.CustomerId, Amount = od.UnitPrice * od.Quantity * (decimal)(1 - od.Discount) })
                .GroupBy(x => x.CustomerId)
                .OrderByDescending(g => g.Sum(x => x.Amount))
                .Take(10)
                .Select(g => g.Key!)
                .ToListAsync();
        }

        public async Task AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
        }

        public async Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Customer customer)
        {
            _context.Customers.Remove(customer);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}