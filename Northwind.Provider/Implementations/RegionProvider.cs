using Northwind.DbAccess.Models;
using Northwind.Provider.Interfaces;
using Northwind.Repository.Interfaces;
using Northwind.ViewModel;
using Northwind.ViewModel.Auth;
using Northwind.ViewModel.Territory;

namespace Northwind.Provider.Implementations
{
    public class RegionProvider : IRegionProvider
    {
        private readonly IRegionRepository _regionRepository;

        public RegionProvider(IRegionRepository regionRepository)
        {
            _regionRepository = regionRepository;
        }

        public async Task<ApiResponse<List<RegionResponseDto>>> GetAllRegionsAsync()
        {
            var regions = await _regionRepository.GetAllAsync();
            var res = regions.Select(r => new RegionResponseDto
            {
                RegionId = r.RegionId,
                RegionDescription = r.RegionDescription.Trim(),
                ListTerritory = r.Territories?.Select(t => new TerritoryResponseDto
                {
                    TerritoryId = t.TerritoryId,
                    TerritoryDescription = t.TerritoryDescription.Trim(),
                    RegionId = t.RegionId
                }).ToList() ?? []
            }).ToList();

            return ApiResponse<List<RegionResponseDto>>.SuccessResult(res);
        }

        public async Task<ApiResponse<PagedResultDto<RegionResponseDto>>> GetPagedRegionsAsync(RegionFilterDto filter)
        {
            var (items, totalCount) = await _regionRepository.GetPagedAsync(filter);
            var dtos = items.Select(r => new RegionResponseDto
            {
                RegionId = r.RegionId,
                RegionDescription = r.RegionDescription.Trim(),
                ListTerritory = r.Territories?.Select(t => new TerritoryResponseDto
                {
                    TerritoryId = t.TerritoryId,
                    TerritoryDescription = t.TerritoryDescription.Trim(),
                    RegionId = t.RegionId
                }).ToList() ?? []
            }).ToList();

            var result = new PagedResultDto<RegionResponseDto>(dtos, totalCount, filter.PageIndex, filter.PageSize);
            return ApiResponse<PagedResultDto<RegionResponseDto>>.SuccessResult(result);
        }

        public async Task<ApiResponse<RegionResponseDto>> GetRegionByIdAsync(int id)
        {
            var r = await _regionRepository.GetByIdAsync(id);
            if (r == null) return ApiResponse<RegionResponseDto>.FailureResult("Region tidak ditemukan.");

            return ApiResponse<RegionResponseDto>.SuccessResult(new RegionResponseDto
            {
                RegionId = r.RegionId,
                RegionDescription = r.RegionDescription.Trim(),
                ListTerritory = r.Territories?.Select(t => new TerritoryResponseDto
                {
                    TerritoryId = t.TerritoryId,
                    TerritoryDescription = t.TerritoryDescription.Trim(),
                    RegionId = t.RegionId
                }).ToList() ?? []
            });
        }

        public async Task<ApiResponse<RegionResponseDto>> CreateRegionAsync(CreateRegionDto req)
        {
            var entity = new Region
            {
                RegionId = req.RegionId,
                RegionDescription = req.RegionDescription
            };

            await _regionRepository.AddAsync(entity);
            await _regionRepository.SaveChangesAsync();

            return ApiResponse<RegionResponseDto>.SuccessResult(
                new RegionResponseDto { RegionId = entity.RegionId, RegionDescription = entity.RegionDescription.Trim() },
                "Region berhasil ditambahkan.");
        }

        public async Task<ApiResponse<bool>> DeleteRegionAsync(int id)
        {
            var r = await _regionRepository.GetByIdAsync(id);
            if (r == null) return ApiResponse<bool>.FailureResult("Region tidak ditemukan.");

            await _regionRepository.DeleteAsync(r);
            await _regionRepository.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResult(true, "Region berhasil dihapus.");
        }
    }
}