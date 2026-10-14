using Northwind.DbAccess.Models;
using Northwind.Provider.Interfaces;
using Northwind.Repository.Interfaces;
using Northwind.ViewModel;
using Northwind.ViewModel.Auth;
using Northwind.ViewModel.Employee;
using Northwind.ViewModel.Territory;

namespace Northwind.Provider.Implementations
{
    public class TerritoryProvider : ITerritoryProvider
    {
        private readonly ITerritoryRepository _territoryRepository;

        public TerritoryProvider(ITerritoryRepository territoryRepository)
        {
            _territoryRepository = territoryRepository;
        }

        public async Task<ApiResponse<List<TerritoryResponseDto>>> GetTerritoriesAsync(int? regionId)
        {
            var list = await _territoryRepository.GetTerritoriesAsync(regionId);
            var res = list.Select(t => new TerritoryResponseDto
            {
                TerritoryId = t.TerritoryId.Trim(),
                TerritoryDescription = t.TerritoryDescription.Trim(),
                RegionId = t.RegionId,
                RegionDescription = t.Region != null ? t.Region.RegionDescription.Trim() : string.Empty,
                Employees = t.Employees?.Select(t => new EmployeeResponseDto
                {
                    EmployeeId = t.EmployeeId,
                    FirstName = t.FirstName,
                    LastName = t.LastName
                }).ToList() ?? []
            }).ToList();

            return ApiResponse<List<TerritoryResponseDto>>.SuccessResult(res);
        }

        public async Task<ApiResponse<PagedResultDto<TerritoryResponseDto>>> GetPagedTerritoriesAsync(TerritoryFilterDto filter)
        {
            var (items, totalCount) = await _territoryRepository.GetPagedTerritoriesAsync(filter);
            var dtos = items.Select(t => new TerritoryResponseDto
            {
                TerritoryId = t.TerritoryId.Trim(),
                TerritoryDescription = t.TerritoryDescription.Trim(),
                RegionId = t.RegionId,
                RegionDescription = t.Region != null ? t.Region.RegionDescription.Trim() : string.Empty,
                Employees = t.Employees?.Select(t => new EmployeeResponseDto
                {
                    EmployeeId = t.EmployeeId,
                    FirstName = t.FirstName,
                    LastName = t.LastName
                }).ToList() ?? []
            }).ToList();

            var result = new PagedResultDto<TerritoryResponseDto>(dtos, totalCount, filter.PageIndex, filter.PageSize);
            return ApiResponse<PagedResultDto<TerritoryResponseDto>>.SuccessResult(result);
        }

        public async Task<ApiResponse<TerritoryResponseDto>> GetTerritoryByIdAsync(string id)
        {
            var t = await _territoryRepository.GetTerritoryByIdAsync(id);
            if (t == null) return ApiResponse<TerritoryResponseDto>.FailureResult("Territory tidak ditemukan.");

            return ApiResponse<TerritoryResponseDto>.SuccessResult(new TerritoryResponseDto
            {
                TerritoryId = t.TerritoryId.Trim(),
                TerritoryDescription = t.TerritoryDescription.Trim(),
                RegionId = t.RegionId,
                RegionDescription = t.Region != null ? t.Region.RegionDescription.Trim() : string.Empty,
                Employees = t.Employees?.Select(t => new EmployeeResponseDto
                {
                    EmployeeId = t.EmployeeId,
                    FirstName = t.FirstName,
                    LastName = t.LastName
                }).ToList() ?? []
            });
        }

        public async Task<ApiResponse<TerritoryResponseDto>> CreateTerritoryAsync(CreateTerritoryDto req)
        {
            var entity = new Territory
            {
                TerritoryId = req.TerritoryId,
                TerritoryDescription = req.TerritoryDescription,
                RegionId = req.RegionId
            };

            await _territoryRepository.AddTerritoryAsync(entity);
            await _territoryRepository.SaveChangesAsync();

            return await GetTerritoryByIdAsync(req.TerritoryId);
        }

        public async Task<ApiResponse<bool>> DeleteTerritoryAsync(string id)
        {
            var t = await _territoryRepository.GetTerritoryByIdAsync(id);
            if (t == null) return ApiResponse<bool>.FailureResult("Territory tidak ditemukan.");

            await _territoryRepository.DeleteTerritoryAsync(t);
            await _territoryRepository.SaveChangesAsync();
            return ApiResponse<bool>.SuccessResult(true, "Territory berhasil dihapus.");
        }
    }
}