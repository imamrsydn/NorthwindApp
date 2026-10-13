using Northwind.ViewModel;
using Northwind.ViewModel.Auth;
using Northwind.ViewModel.Shipper;

namespace Northwind.Provider.Interfaces
{
    public interface IShipperProvider
    {
        Task<ApiResponse<List<ShipperResponseDto>>> GetAllShippersAsync();
        Task<ApiResponse<PagedResultDto<ShipperResponseDto>>> GetPagedShippersAsync(ShipperFilterDto filter);
        Task<ApiResponse<ShipperResponseDto>> GetShipperByIdAsync(int id);
        Task<ApiResponse<ShipperResponseDto>> CreateShipperAsync(CreateShipperDto request);
        Task<ApiResponse<ShipperResponseDto>> UpdateShipperAsync(UpdateShipperDto request);
        Task<ApiResponse<bool>> DeleteShipperAsync(int id);
    }
}