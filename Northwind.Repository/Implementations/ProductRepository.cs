using Microsoft.EntityFrameworkCore;
using Northwind.DbAccess.Models;
using Northwind.Repository.Interfaces;
using Northwind.ViewModel.Product;

namespace Northwind.Repository.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly Northwind3Context _context;

        public ProductRepository(Northwind3Context context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync(int? categoryId, int? supplierId, bool? discontinued, bool? readyToReorder)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (supplierId.HasValue)
                query = query.Where(p => p.SupplierId == supplierId.Value);

            if (discontinued.HasValue)
                query = query.Where(p => p.Discontinued == discontinued.Value);

            if (readyToReorder.HasValue && readyToReorder.Value)
                query = query.Where(p => ((p.UnitsInStock ?? 0) + (p.UnitsOnOrder ?? 0)) <= (p.ReorderLevel ?? 0) && !p.Discontinued);

            return await query.ToListAsync();
        }

        public async Task<(List<Product> Items, int TotalCount)> GetPagedAsync(ProductFilterDto filter)
        {
            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchKeyword))
            {
                var keyword = filter.SearchKeyword.Trim().ToLower();
                query = query.Where(p => p.ProductName.ToLower().Contains(keyword));
            }

            if (filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

            if (filter.SupplierId.HasValue)
                query = query.Where(p => p.SupplierId == filter.SupplierId.Value);

            if (filter.Discontinued.HasValue)
                query = query.Where(p => p.Discontinued == filter.Discontinued.Value);

            if (filter.ReadyToOrder.HasValue && filter.ReadyToOrder.Value)
                query = query.Where(p => ((p.UnitsInStock ?? 0) + (p.UnitsOnOrder ?? 0)) <= (p.ReorderLevel ?? 0) && !p.Discontinued);

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.ProductId)
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<Product>> GetAllForReorderAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => !p.Discontinued)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsWithUnitsOnOrderAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => (p.UnitsOnOrder ?? 0) > 0)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}