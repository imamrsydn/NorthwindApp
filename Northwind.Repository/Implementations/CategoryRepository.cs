using Microsoft.EntityFrameworkCore;
using Northwind.DbAccess.Models;
using Northwind.Repository.Interfaces;
using Northwind.ViewModel.Category;

namespace Northwind.Repository.Implementations;

public class CategoryRepository : ICategoryRepository
{
    private readonly Northwind3Context _context;

    public CategoryRepository(Northwind3Context context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetAllAsync(string? nameFilter)
    {
        var query = _context.Categories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(nameFilter))
        {
            query = query.Where(c => c.CategoryName.Contains(nameFilter));
        }

        return await query.ToListAsync();
    }

    public async Task<(List<Category> Items, int TotalCount)> GetPagedAsync(CategoryFilterDto filter)
    {
        var query = _context.Categories.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.SearchKeyword))
        {
            var keyword = filter.SearchKeyword.Trim().ToLower();
            query = query.Where(c => c.CategoryName.ToLower().Contains(keyword) ||
                                     (c.Description != null && c.Description.Contains(keyword)));
        }

        int totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(c => c.CategoryId)
            .Skip((filter.PageIndex - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task<Category?> GetByIdWithProductsAsync(int id)
    {
        return await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.CategoryId == id);
    }

    public async Task AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
    }

    public async Task UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Category category)
    {
        _context.Categories.Remove(category);
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Categories.AnyAsync(c => c.CategoryId == id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<Tuple<int, string, int, decimal>>> GetTopProductsAsync(int limit, int categoryId)
    {
        var top = await _context.OrderDetails
            .Include(od => od.Product)
            .Where(od => od.Product.CategoryId == categoryId)
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
}