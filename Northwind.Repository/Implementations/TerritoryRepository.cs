using Microsoft.EntityFrameworkCore;
using Northwind.DbAccess.Models;
using Northwind.Repository.Interfaces;
using Northwind.ViewModel.Territory;

namespace Northwind.Repository.Implementations
{
    public class TerritoryRepository : ITerritoryRepository
    {
        private readonly Northwind3Context _context;

        public TerritoryRepository(Northwind3Context context)
        {
            _context = context;
        }

        public async Task<List<Territory>> GetTerritoriesAsync(int? regionId)
        {
            var q = _context.Territories.AsNoTracking().Include(t => t.Region).AsQueryable();
            if (regionId.HasValue) q = q.Where(t => t.RegionId == regionId.Value);
            return await q.ToListAsync();
        }

        public async Task<(List<Territory> Items, int TotalCount)> GetPagedTerritoriesAsync(TerritoryFilterDto filter)
        {
            var query = _context.Territories
                .AsNoTracking()
                .Include(t => t.Region)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchKeyword))
            {
                var keyword = filter.SearchKeyword.Trim().ToLower();
                query = query.Where(t => t.TerritoryId.ToLower().Contains(keyword) ||
                                         t.TerritoryDescription.ToLower().Contains(keyword));
            }

            if (filter.RegionId.HasValue)
            {
                query = query.Where(t => t.RegionId == filter.RegionId.Value);
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(t => t.TerritoryId)
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Territory?> GetTerritoryByIdAsync(string id)
        {
            return await _context.Territories
                .AsNoTracking()
                .Include(t => t.Region)
                .FirstOrDefaultAsync(t => t.TerritoryId.Trim() == id.Trim());
        }

        public async Task AddTerritoryAsync(Territory territory)
        {
            await _context.Territories.AddAsync(territory);
        }

        public async Task UpdateTerritoryAsync(Territory territory)
        {
            _context.Territories.Update(territory);
            await Task.CompletedTask;
        }

        public async Task DeleteTerritoryAsync(Territory territory)
        {
            _context.Territories.Remove(territory);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}