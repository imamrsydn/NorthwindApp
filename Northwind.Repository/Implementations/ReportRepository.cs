using Microsoft.EntityFrameworkCore;
using Northwind.DbAccess.Models;
using Northwind.Repository.Interfaces;

namespace Northwind.Repository.Implementations
{
    public class ReportRepository : IReportRepository
    {
        private readonly Northwind3Context _context;

        public ReportRepository(Northwind3Context context) { _context = context; }

        public async Task<List<Tuple<string, int, decimal>>> GetSalesByMonthAsync(int year)
        {
            var data = await _context.Orders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == year)
                .Include(o => o.OrderDetails)
                .GroupBy(o => o.OrderDate!.Value.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    OrderCount = g.Count(),
                    TotalSales = g.SelectMany(o => o.OrderDetails).Sum(od => od.UnitPrice * od.Quantity * (decimal)(1 - od.Discount))
                })
                .OrderBy(x => x.Month)
                .ToListAsync();

            return data.Select(x => Tuple.Create($"Month-{x.Month}", x.OrderCount, x.TotalSales)).ToList();
        }

        public async Task<List<Tuple<string, int, decimal>>> GetSalesByProductAsync()
        {
            var data = await _context.OrderDetails
                .Include(od => od.Product)
                .GroupBy(od => od.Product.ProductName)
                .Select(g => new
                {
                    ProductName = g.Key,
                    OrderCount = g.Count(),
                    TotalSales = g.Sum(od => od.UnitPrice * od.Quantity * (decimal)(1 - od.Discount))
                })
                .OrderByDescending(x => x.TotalSales)
                .ToListAsync();

            return data.Select(x => Tuple.Create(x.ProductName, x.OrderCount, x.TotalSales)).ToList();
        }

        public async Task<List<Tuple<string, int, decimal>>> GetSalesByCategoryAsync()
        {
            var data = await _context.OrderDetails
                .Include(od => od.Product).ThenInclude(p => p.Category)
                .GroupBy(od => od.Product.Category.CategoryName)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    OrderCount = g.Count(),
                    TotalSales = g.Sum(od => od.UnitPrice * od.Quantity * (decimal)(1 - od.Discount))
                })
                .OrderByDescending(x => x.TotalSales)
                .ToListAsync();

            return data.Select(x => Tuple.Create(x.CategoryName, x.OrderCount, x.TotalSales)).ToList();
        }

        public async Task<List<Tuple<string, int, decimal>>> GetSalesByCustomerAsync()
        {
            var data = await _context.Orders
                .Where(o => o.CustomerId != null)
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .GroupBy(o => o.Customer.CompanyName)
                .Select(g => new
                {
                    CompanyName = g.Key,
                    OrderCount = g.Count(),
                    TotalSales = g.SelectMany(o => o.OrderDetails).Sum(od => od.UnitPrice * od.Quantity * (decimal)(1 - od.Discount))
                })
                .OrderByDescending(x => x.TotalSales)
                .ToListAsync();

            return data.Select(x => Tuple.Create(x.CompanyName, x.OrderCount, x.TotalSales)).ToList();
        }

        public async Task<List<Tuple<string, int, decimal>>> GetSalesByEmployeeAsync()
        {
            var data = await _context.Orders
                .Where(o => o.EmployeeId != null)
                .Include(o => o.Employee)
                .Include(o => o.OrderDetails)
                .GroupBy(o => o.Employee.FirstName + " " + o.Employee.LastName)
                .Select(g => new
                {
                    EmployeeName = g.Key,
                    OrderCount = g.Count(),
                    TotalSales = g.SelectMany(o => o.OrderDetails).Sum(od => od.UnitPrice * od.Quantity * (decimal)(1 - od.Discount))
                })
                .OrderByDescending(x => x.TotalSales)
                .ToListAsync();

            return data.Select(x => Tuple.Create(x.EmployeeName, x.OrderCount, x.TotalSales)).ToList();
        }
    }
}