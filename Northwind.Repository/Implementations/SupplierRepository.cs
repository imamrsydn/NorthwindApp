using Microsoft.EntityFrameworkCore;
using Northwind.DbAccess.Models;
using Northwind.Repository.Interfaces;
using Northwind.ViewModel.Supplier;

namespace Northwind.Repository.Implementations
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly Northwind3Context _context;

        public SupplierRepository(Northwind3Context context)
        {
            _context = context;
        }

        public async Task<List<Supplier>> GetAllAsync(string? name, string? city, string? country)
        {
            var query = _context.Suppliers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(s => s.CompanyName.Contains(name));

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(s => s.City != null && s.City.Contains(city));

            if (!string.IsNullOrWhiteSpace(country))
                query = query.Where(s => s.Country != null && s.Country.Contains(country));

            return await query.ToListAsync();
        }

        public async Task<(List<Supplier> Items, int TotalCount)> GetPagedAsync(SupplierFilterDto filter)
        {
            var query = _context.Suppliers.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchKeyword))
            {
                var keyword = filter.SearchKeyword.Trim().ToLower();
                query = query.Where(s => s.CompanyName.ToLower().Contains(keyword) ||
                                         (s.ContactName != null && s.ContactName.ToLower().Contains(keyword)));
            }

            if (!string.IsNullOrWhiteSpace(filter.CompanyName))
                query = query.Where(s => s.CompanyName.Contains(filter.CompanyName));

            if (!string.IsNullOrWhiteSpace(filter.Country))
                query = query.Where(s => s.Country != null && s.Country.Contains(filter.Country));

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(s => s.SupplierId)
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Supplier?> GetByIdAsync(int id)
        {
            return await _context.Suppliers.FindAsync(id);
        }

        public async Task<Supplier?> GetByIdWithProductsAsync(int id)
        {
            return await _context.Suppliers
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.SupplierId == id);
        }

        public async Task AddAsync(Supplier supplier)
        {
            await _context.Suppliers.AddAsync(supplier);
        }

        public async Task UpdateAsync(Supplier supplier)
        {
            _context.Suppliers.Update(supplier);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Supplier supplier)
        {
            _context.Suppliers.Remove(supplier);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}