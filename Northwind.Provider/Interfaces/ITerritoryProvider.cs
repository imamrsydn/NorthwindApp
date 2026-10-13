using Northwind.ViewModel;
using Northwind.ViewModel.Auth;
using Northwind.ViewModel.Territory;

namespace Northwind.Provider.Interfaces
{
    public interface ITerritoryProvider
    {
        Task<ApiResponse<List<TerritoryResponseDto>>> GetTerritoriesAsync(int? regionId);
        Task<ApiResponse<PagedResultDto<TerritoryResponseDto>>> GetPagedTerritoriesAsync(TerritoryFilterDto filter);
        Task<ApiResponse<TerritoryResponseDto>> GetTerritoryByIdAsync(string id);
        Task<ApiResponse<TerritoryResponseDto>> CreateTerritoryAsync(CreateTerritoryDto request);
        Task<ApiResponse<bool>> DeleteTerritoryAsync(string id);
    }
}