using Northwind.DbAccess.Models;
using Northwind.ViewModel.Territory;

namespace Northwind.Repository.Interfaces
{
    public interface ITerritoryRepository
    {
        Task<List<Territory>> GetTerritoriesAsync(int? regionId);
        Task<(List<Territory> Items, int TotalCount)> GetPagedTerritoriesAsync(TerritoryFilterDto filter);
        Task<Territory?> GetTerritoryByIdAsync(string id);
        Task AddTerritoryAsync(Territory territory);
        Task UpdateTerritoryAsync(Territory territory);
        Task DeleteTerritoryAsync(Territory territory);
        Task SaveChangesAsync();
    }
}