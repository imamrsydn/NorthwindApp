using Northwind.ViewModel;
using Northwind.ViewModel.Auth;
using Northwind.ViewModel.Supplier;

namespace Northwind.Provider.Interfaces
{
    public interface ISupplierProvider
    {
        Task<ApiResponse<List<SupplierResponseDto>>> GetAllSuppliersAsync(string? name, string? city, string? country);
        Task<ApiResponse<PagedResultDto<SupplierResponseDto>>> GetPagedSuppliersAsync(SupplierFilterDto filter);
        Task<ApiResponse<SupplierDetailDto>> GetSupplierByIdAsync(int id);
        Task<ApiResponse<SupplierResponseDto>> CreateSupplierAsync(CreateSupplierDto request);
        Task<ApiResponse<SupplierResponseDto>> UpdateSupplierAsync(UpdateSupplierDto request);
        Task<ApiResponse<bool>> DeleteSupplierAsync(int id);
    }
}