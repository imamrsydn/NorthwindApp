using Microsoft.EntityFrameworkCore;
using Northwind.DbAccess.Models;
using Northwind.Repository.Interfaces;

namespace Northwind.Repository.Implementations
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly Northwind3Context _context;

        public DashboardRepository(Northwind3Context context)
        {
            _context = context;
        }

        public async Task<int> GetTotalProductsAsync() => await _context.Products.CountAsync();
        public async Task<int> GetActiveProductsAsync() => await _context.Products.CountAsync(p => !p.Discontinued);
        public async Task<int> GetTotalCustomersAsync() => await _context.Customers.CountAsync();
        public async Task<int> GetTotalEmployeesAsync() => await _context.Employees.CountAsync();
        public async Task<int> GetTotalOrdersAsync() => await _context.Orders.CountAsync();
        public async Task<int> GetPendingOrdersAsync() => await _context.Orders.CountAsync(o => o.ShippedDate == null);
        public async Task<int> GetShippedOrdersAsync() => await _context.Orders.CountAsync(o => o.ShippedDate != null);

        public async Task<decimal> GetTotalSalesAsync()
        {
            return await _context.OrderDetails
                .SumAsync(od => od.UnitPrice * od.Quantity * (decimal)(1 - od.Discount));
        }

        public async Task<List<Tuple<int, string, int, decimal>>> GetTopProductsAsync(int limit)
        {
            var top = await _context.OrderDetails
                .Include(od => od.Product)
                .GroupBy(od => new { od.ProductId, od.Product.ProductName })
                .Select(g => new
                {
                    g.Key.ProductId,
                    g.Key.ProductName,
                    QtySold = g.Sum(x => x.Quantity),
                    Revenue = g.Sum(x => x.UnitPrice * x.Quantity * (decimal)(1 - x.Discount))
                })
                .OrderByDescending(x => x.QtySold)
                .Take(limit)
                .ToListAsync();

            return top.Select(x => Tuple.Create(x.ProductId, x.ProductName, (int)x.QtySold, x.Revenue)).ToList();
        }

        public async Task<List<Tuple<string, string, int, decimal>>> GetTopCustomersAsync(int limit)
        {
            var top = await _context.Orders
                .Where(o => o.CustomerId != null)
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .GroupBy(o => new { o.CustomerId, o.Customer.CompanyName })
                .Select(g => new
                {
                    g.Key.CustomerId,
                    g.Key.CompanyName,
                    RepeatOrders = g.Count(),
                    TotalSpent = g.SelectMany(o => o.OrderDetails).Sum(od => od.UnitPrice * od.Quantity * (decimal)(1 - od.Discount))
                })
                .OrderByDescending(x => x.RepeatOrders)
                .Take(limit)
                .ToListAsync();

            return top.Select(x => Tuple.Create(x.CustomerId!, x.CompanyName, x.RepeatOrders, x.TotalSpent)).ToList();
        }

        public async Task<List<Order>> GetOverdueOrdersAsync()
        {
            // Telat lebih dari 3 hari dari tanggal seharusnya diterima customer (RequiredDate)
            var limitDate = DateTime.UtcNow.AddDays(-3);

            return await _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.ShippedDate == null && o.RequiredDate.HasValue && o.RequiredDate.Value < limitDate)
                .ToListAsync();
        }
    }
}