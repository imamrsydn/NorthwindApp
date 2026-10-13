using Northwind.DbAccess.Models;
using Northwind.Provider.Interfaces;
using Northwind.Repository.Interfaces;
using Northwind.ViewModel;
using Northwind.ViewModel.Auth;
using Northwind.ViewModel.Category;
using Northwind.ViewModel.Dashboard;

namespace Northwind.Provider.Implementations;

public class CategoryProvider : ICategoryProvider
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryProvider(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ApiResponse<List<CategoryResponseDto>>> GetAllCategoriesAsync(string? nameFilter)
    {
        var categories = await _categoryRepository.GetAllAsync(nameFilter);

        var response = categories.Select(c => new CategoryResponseDto
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName,
            Description = c.Description
        }).ToList();

        return ApiResponse<List<CategoryResponseDto>>.SuccessResult(response);
    }

    public async Task<ApiResponse<PagedResultDto<CategoryResponseDto>>> GetPagedCategoriesAsync(CategoryFilterDto filter)
    {
        var (items, totalCount) = await _categoryRepository.GetPagedAsync(filter);

        var dtos = items.Select(c => new CategoryResponseDto
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName,
            Description = c.Description
        }).ToList();

        var result = new PagedResultDto<CategoryResponseDto>(dtos, totalCount, filter.PageIndex, filter.PageSize);
        return ApiResponse<PagedResultDto<CategoryResponseDto>>.SuccessResult(result);
    }

    public async Task<ApiResponse<CategoryDetailDto>> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdWithProductsAsync(id);

        if (category == null)
        {
            return ApiResponse<CategoryDetailDto>.FailureResult("Kategori tidak ditemukan.");
        }

        var response = new CategoryDetailDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            Description = category.Description,
            Products = [.. category.Products.Select(p => new CategoryProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                UnitPrice = p.UnitPrice,
                UnitsInStock = p.UnitsInStock,
                Discontinued = p.Discontinued
            })]
        };

        return ApiResponse<CategoryDetailDto>.SuccessResult(response);
    }

    public async Task<ApiResponse<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryDto request)
    {
        var category = new Category
        {
            CategoryName = request.CategoryName,
            Description = request.Description
        };

        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();

        var response = new CategoryResponseDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            Description = category.Description
        };

        return ApiResponse<CategoryResponseDto>.SuccessResult(response, "Kategori berhasil ditambahkan.");
    }

    public async Task<ApiResponse<CategoryResponseDto>> UpdateCategoryAsync(UpdateCategoryDto request)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null)
        {
            return ApiResponse<CategoryResponseDto>.FailureResult("Kategori tidak ditemukan.");
        }

        category.CategoryName = request.CategoryName;
        category.Description = request.Description;

        await _categoryRepository.UpdateAsync(category);
        await _categoryRepository.SaveChangesAsync();

        var response = new CategoryResponseDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            Description = category.Description
        };

        return ApiResponse<CategoryResponseDto>.SuccessResult(response, "Kategori berhasil diperbarui.");
    }

    public async Task<ApiResponse<bool>> DeleteCategoryAsync(int id)
    {
        var category = await _categoryRepository.GetByIdWithProductsAsync(id);
        if (category == null)
        {
            return ApiResponse<bool>.FailureResult("Kategori tidak ditemukan.");
        }

        if (category.Products.Count != 0)
        {
            return ApiResponse<bool>.FailureResult("Kategori tidak dapat dihapus karena masih digunakan oleh produk.");
        }

        await _categoryRepository.DeleteAsync(category);
        await _categoryRepository.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResult(true, "Kategori berhasil dihapus.");
    }

    public async Task<ApiResponse<List<TopProductDto>>> GetTopProduct(int limit, int id)
    {
        var topProduct = await _categoryRepository.GetTopProductsAsync(limit, id);
        var result = topProduct.Select(x => new TopProductDto
        {
            ProductId = x.Item1,
            ProductName = x.Item2,
            QuantitySold = x.Item3,
            TotalRevenue = Math.Round(x.Item4, 2)
        }).ToList();

        return ApiResponse<List<TopProductDto>>.SuccessResult(result);
    }
}