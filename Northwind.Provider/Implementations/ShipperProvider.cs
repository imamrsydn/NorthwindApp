using Northwind.DbAccess.Models;
using Northwind.Provider.Interfaces;
using Northwind.Repository.Interfaces;
using Northwind.ViewModel;
using Northwind.ViewModel.Auth;
using Northwind.ViewModel.Shipper;

namespace Northwind.Provider.Implementations
{
    public class ShipperProvider : IShipperProvider
    {
        private readonly IShipperRepository _shipperRepository;

        public ShipperProvider(IShipperRepository shipperRepository)
        {
            _shipperRepository = shipperRepository;
        }

        public async Task<ApiResponse<List<ShipperResponseDto>>> GetAllShippersAsync()
        {
            var shippers = await _shipperRepository.GetAllAsync();
            var response = shippers.Select(s => new ShipperResponseDto { ShipperId = s.ShipperId, CompanyName = s.CompanyName, Phone = s.Phone }).ToList();
            return ApiResponse<List<ShipperResponseDto>>.SuccessResult(response);
        }

        public async Task<ApiResponse<PagedResultDto<ShipperResponseDto>>> GetPagedShippersAsync(ShipperFilterDto filter)
        {
            var (items, totalCount) = await _shipperRepository.GetPagedAsync(filter);

            var dtos = items.Select(s => new ShipperResponseDto
            {
                ShipperId = s.ShipperId,
                CompanyName = s.CompanyName,
                Phone = s.Phone
            }).ToList();

            var result = new PagedResultDto<ShipperResponseDto>(dtos, totalCount, filter.PageIndex, filter.PageSize);
            return ApiResponse<PagedResultDto<ShipperResponseDto>>.SuccessResult(result);
        }

        public async Task<ApiResponse<ShipperResponseDto>> GetShipperByIdAsync(int id)
        {
            var s = await _shipperRepository.GetByIdAsync(id);
            if (s == null) return ApiResponse<ShipperResponseDto>.FailureResult("Shipper tidak ditemukan.");
            return ApiResponse<ShipperResponseDto>.SuccessResult(new ShipperResponseDto { ShipperId = s.ShipperId, CompanyName = s.CompanyName, Phone = s.Phone });
        }

        public async Task<ApiResponse<ShipperResponseDto>> CreateShipperAsync(CreateShipperDto request)
        {
            var shipper = new Shipper { CompanyName = request.CompanyName, Phone = request.Phone };
            await _shipperRepository.AddAsync(shipper);
            await _shipperRepository.SaveChangesAsync();
            return ApiResponse<ShipperResponseDto>.SuccessResult(new ShipperResponseDto { ShipperId = shipper.ShipperId, CompanyName = shipper.CompanyName, Phone = shipper.Phone }, "Shipper berhasil ditambahkan.");
        }

        public async Task<ApiResponse<ShipperResponseDto>> UpdateShipperAsync(UpdateShipperDto request)
        {
            var shipper = await _shipperRepository.GetByIdAsync(request.ShipperId);
            if (shipper == null) return ApiResponse<ShipperResponseDto>.FailureResult("Shipper tidak ditemukan.");

            shipper.CompanyName = request.CompanyName;
            shipper.Phone = request.Phone;

            await _shipperRepository.UpdateAsync(shipper);
            await _shipperRepository.SaveChangesAsync();
            return ApiResponse<ShipperResponseDto>.SuccessResult(new ShipperResponseDto { ShipperId = shipper.ShipperId, CompanyName = shipper.CompanyName, Phone = shipper.Phone }, "Shipper berhasil diperbarui.");
        }

        public async Task<ApiResponse<bool>> DeleteShipperAsync(int id)
        {
            var shipper = await _shipperRepository.GetByIdAsync(id);
            if (shipper == null) return ApiResponse<bool>.FailureResult("Shipper tidak ditemukan.");

            await _shipperRepository.DeleteAsync(shipper);
            await _shipperRepository.SaveChangesAsync();
            return ApiResponse<bool>.SuccessResult(true, "Shipper berhasil dihapus.");
        }
    }
}