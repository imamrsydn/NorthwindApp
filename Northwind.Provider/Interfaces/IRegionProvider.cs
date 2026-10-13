using Northwind.ViewModel;
using Northwind.ViewModel.Auth;
using Northwind.ViewModel.Territory;

namespace Northwind.Provider.Interfaces
{
    public interface IRegionProvider
    {
        Task<ApiResponse<List<RegionResponseDto>>> GetAllRegionsAsync();
        Task<ApiResponse<PagedResultDto<RegionResponseDto>>> GetPagedRegionsAsync(RegionFilterDto filter);
        Task<ApiResponse<RegionResponseDto>> GetRegionByIdAsync(int id);
        Task<ApiResponse<RegionResponseDto>> CreateRegionAsync(CreateRegionDto request);
        Task<ApiResponse<bool>> DeleteRegionAsync(int id);
    }
}