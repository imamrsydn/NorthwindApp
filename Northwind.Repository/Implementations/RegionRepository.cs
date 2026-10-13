using Microsoft.EntityFrameworkCore;
using Northwind.DbAccess.Models;
using Northwind.Repository.Interfaces;
using Northwind.ViewModel.Territory;

namespace Northwind.Repository.Implementations
{
    public class RegionRepository : IRegionRepository
    {
        private readonly Northwind3Context _context;

        public RegionRepository(Northwind3Context context)
        {
            _context = context;
        }

        public async Task<List<Region>> GetAllAsync()
        {
            return await _context.Regions.AsNoTracking().ToListAsync();
        }

        public async Task<(List<Region> Items, int TotalCount)> GetPagedAsync(RegionFilterDto filter)
        {
            var query = _context.Regions.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchKeyword))
            {
                var keyword = filter.SearchKeyword.Trim().ToLower();
                query = query.Where(r => r.RegionDescription.ToLower().Contains(keyword));
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(r => r.RegionId)
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Region?> GetByIdAsync(int id)
        {
            return await _context.Regions.FindAsync(id);
        }

        public async Task AddAsync(Region region)
        {
            await _context.Regions.AddAsync(region);
        }

        public async Task UpdateAsync(Region region)
        {
            _context.Regions.Update(region);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Region region)
        {
            _context.Regions.Remove(region);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}