using Northwind.ViewModel;
using Northwind.ViewModel.Auth;
using Northwind.ViewModel.Product;

namespace Northwind.Provider.Interfaces
{
    public interface IProductProvider
    {
        Task<ApiResponse<List<ProductResponseDto>>> GetAllProductsAsync(int? categoryId, int? supplierId, bool? discontinued, bool? readyToReorder);
        Task<ApiResponse<PagedResultDto<ProductResponseDto>>> GetPagedProductsAsync(ProductFilterDto filter);
        Task<ApiResponse<ProductResponseDto>> GetProductByIdAsync(int id);
        Task<ApiResponse<ProductResponseDto>> CreateProductAsync(CreateProductDto request);
        Task<ApiResponse<ProductResponseDto>> UpdateProductAsync(UpdateProductDto request);
        Task<ApiResponse<bool>> DeleteProductAsync(int id);
        Task<ApiResponse<bool>> ToggleDiscontinueAsync(int id, bool discontinued);
        Task<ApiResponse<List<ProductResponseDto>>> ReorderProductAsync(ReorderProductDto request);
        Task<ApiResponse<List<ProductResponseDto>>> ReceiveStockFromOrderAsync();
    }
}