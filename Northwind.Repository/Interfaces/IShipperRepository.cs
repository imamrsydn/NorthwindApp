using Northwind.DbAccess.Models;
using Northwind.ViewModel.Shipper;

namespace Northwind.Repository.Interfaces
{
    public interface IShipperRepository
    {
        Task<List<Shipper>> GetAllAsync();
        Task<(List<Shipper> Items, int TotalCount)> GetPagedAsync(ShipperFilterDto filter);
        Task<Shipper?> GetByIdAsync(int id);
        Task AddAsync(Shipper shipper);
        Task UpdateAsync(Shipper shipper);
        Task DeleteAsync(Shipper shipper);
        Task SaveChangesAsync();
    }
}