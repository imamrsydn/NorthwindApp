using Northwind.DbAccess.Models;
using Northwind.ViewModel.Territory;

namespace Northwind.Repository.Interfaces
{
    public interface IRegionRepository
    {
        Task<List<Region>> GetAllAsync();
        Task<(List<Region> Items, int TotalCount)> GetPagedAsync(RegionFilterDto filter);
        Task<Region?> GetByIdAsync(int id);
        Task AddAsync(Region region);
        Task UpdateAsync(Region region);
        Task DeleteAsync(Region region);
        Task SaveChangesAsync();
    }
}