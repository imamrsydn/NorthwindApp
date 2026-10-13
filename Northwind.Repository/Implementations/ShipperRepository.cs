using Microsoft.EntityFrameworkCore;
using Northwind.DbAccess.Models;
using Northwind.Repository.Interfaces;
using Northwind.ViewModel.Shipper;

namespace Northwind.Repository.Implementations
{
    public class ShipperRepository : IShipperRepository
    {
        private readonly Northwind3Context _context;

        public ShipperRepository(Northwind3Context context)
        {
            _context = context;
        }

        public async Task<List<Shipper>> GetAllAsync() => await _context.Shippers.ToListAsync();
        public async Task<(List<Shipper> Items, int TotalCount)> GetPagedAsync(ShipperFilterDto filter)
        {
            var query = _context.Shippers.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchKeyword))
            {
                var keyword = filter.SearchKeyword.Trim().ToLower();
                query = query.Where(r => r.CompanyName.ToLower().Contains(keyword));
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(r => r.ShipperId)
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Shipper?> GetByIdAsync(int id) => await _context.Shippers.FindAsync(id);
        public async Task AddAsync(Shipper shipper) => await _context.Shippers.AddAsync(shipper);
        public async Task UpdateAsync(Shipper shipper) { _context.Shippers.Update(shipper); await Task.CompletedTask; }
        public async Task DeleteAsync(Shipper shipper) { _context.Shippers.Remove(shipper); await Task.CompletedTask; }
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}