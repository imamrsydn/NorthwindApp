using Northwind.ViewModel;
using Northwind.ViewModel.Auth;
using Northwind.ViewModel.Category;
using Northwind.ViewModel.Dashboard;

namespace Northwind.Provider.Interfaces;

public interface ICategoryProvider
{
    Task<ApiResponse<List<CategoryResponseDto>>> GetAllCategoriesAsync(string? nameFilter);
    Task<ApiResponse<PagedResultDto<CategoryResponseDto>>> GetPagedCategoriesAsync(CategoryFilterDto filter);
    Task<ApiResponse<CategoryDetailDto>> GetCategoryByIdAsync(int id);
    Task<ApiResponse<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryDto request);
    Task<ApiResponse<CategoryResponseDto>> UpdateCategoryAsync(UpdateCategoryDto request);
    Task<ApiResponse<bool>> DeleteCategoryAsync(int id);
    Task<ApiResponse<List<TopProductDto>>> GetTopProduct(int limit, int id);
}