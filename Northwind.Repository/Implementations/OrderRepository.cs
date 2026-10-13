using Microsoft.EntityFrameworkCore;
using Northwind.DbAccess.Models;
using Northwind.Repository.Interfaces;
using Northwind.ViewModel.Order;

namespace Northwind.Repository.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly Northwind3Context _context;

        public OrderRepository(Northwind3Context context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync(string? customerId, int? supplierId, int? employeeId, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.ShipViaNavigation)
                .Include(o => o.OrderDetails)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(customerId))
                query = query.Where(o => o.CustomerId == customerId);

            if (employeeId.HasValue)
                query = query.Where(o => o.EmployeeId == employeeId.Value);

            if (supplierId.HasValue)
                query = query.Where(o => o.OrderDetails.Any(od => od.Product.SupplierId == supplierId.Value));

            if (startDate.HasValue)
                query = query.Where(o => o.OrderDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(o => o.OrderDate <= endDate.Value);

            return await query.OrderByDescending(o => o.OrderDate).ToListAsync();
        }

        public async Task<(List<Order> Items, int TotalCount)> GetPagedAsync(OrderFilterDto filter)
        {
            var query = _context.Orders
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.ShipViaNavigation)
                .Include(o => o.OrderDetails)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchKeyword))
            {
                var keyword = filter.SearchKeyword.Trim().ToLower();
                query = query.Where(o => o.OrderId.ToString().Contains(keyword) ||
                                         (o.ShipName != null && o.ShipName.ToLower().Contains(keyword)));
            }

            if (!string.IsNullOrWhiteSpace(filter.CustomerId))
                query = query.Where(o => o.CustomerId == filter.CustomerId);

            if (filter.EmployeeId.HasValue)
                query = query.Where(o => o.EmployeeId == filter.EmployeeId.Value);

            if (filter.SupplierId.HasValue)
                query = query.Where(o => o.OrderDetails.Any(od => od.Product != null && od.Product.SupplierId == filter.SupplierId.Value));

            if (filter.StartDate.HasValue)
                query = query.Where(o => o.OrderDate >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(o => o.OrderDate <= filter.EndDate.Value);

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(o => o.OrderDate) // Order terbaru di atas
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task<Order?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.ShipViaNavigation)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Order order)
        {
            _context.Orders.Remove(order);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}