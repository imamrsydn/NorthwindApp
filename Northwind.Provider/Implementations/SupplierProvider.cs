using Northwind.DbAccess.Models;
using Northwind.Provider.Interfaces;
using Northwind.Repository.Interfaces;
using Northwind.ViewModel;
using Northwind.ViewModel.Auth;
using Northwind.ViewModel.Supplier;

namespace Northwind.Provider.Implementations;

public class SupplierProvider : ISupplierProvider
{
    private readonly ISupplierRepository _supplierRepository;

    public SupplierProvider(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<ApiResponse<List<SupplierResponseDto>>> GetAllSuppliersAsync(string? name, string? city, string? country)
    {
        var suppliers = await _supplierRepository.GetAllAsync(name, city, country);

        var response = suppliers.Select(MapToResponseDto).ToList();
        return ApiResponse<List<SupplierResponseDto>>.SuccessResult(response);
    }

    public async Task<ApiResponse<PagedResultDto<SupplierResponseDto>>> GetPagedSuppliersAsync(SupplierFilterDto filter)
    {
        var (items, totalCount) = await _supplierRepository.GetPagedAsync(filter);
        var dtos = items.Select(MapToResponseDto).ToList();

        var result = new PagedResultDto<SupplierResponseDto>(dtos, totalCount, filter.PageIndex, filter.PageSize);
        return ApiResponse<PagedResultDto<SupplierResponseDto>>.SuccessResult(result);
    }

    public async Task<ApiResponse<SupplierDetailDto>> GetSupplierByIdAsync(int id)
    {
        var supplier = await _supplierRepository.GetByIdWithProductsAsync(id);
        if (supplier == null)
        {
            return ApiResponse<SupplierDetailDto>.FailureResult("Supplier tidak ditemukan.");
        }

        var response = new SupplierDetailDto
        {
            SupplierId = supplier.SupplierId,
            CompanyName = supplier.CompanyName,
            ContactName = supplier.ContactName,
            ContactTitle = supplier.ContactTitle,
            Address = supplier.Address,
            City = supplier.City,
            Region = supplier.Region,
            PostalCode = supplier.PostalCode,
            Country = supplier.Country,
            Phone = supplier.Phone,
            Fax = supplier.Fax,
            HomePage = supplier.HomePage,
            Products = [.. supplier.Products.Select(p => new SupplierProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                UnitPrice = p.UnitPrice,
                UnitsInStock = p.UnitsInStock,
                Discontinued = p.Discontinued
            })]
        };

        return ApiResponse<SupplierDetailDto>.SuccessResult(response);
    }

    public async Task<ApiResponse<SupplierResponseDto>> CreateSupplierAsync(CreateSupplierDto request)
    {
        var supplier = new Supplier
        {
            CompanyName = request.CompanyName,
            ContactName = request.ContactName,
            ContactTitle = request.ContactTitle,
            Address = request.Address,
            City = request.City,
            Region = request.Region,
            PostalCode = request.PostalCode,
            Country = request.Country,
            Phone = request.Phone,
            Fax = request.Fax,
            HomePage = request.HomePage
        };

        await _supplierRepository.AddAsync(supplier);
        await _supplierRepository.SaveChangesAsync();

        return ApiResponse<SupplierResponseDto>.SuccessResult(MapToResponseDto(supplier), "Supplier berhasil ditambahkan.");
    }

    public async Task<ApiResponse<SupplierResponseDto>> UpdateSupplierAsync(UpdateSupplierDto request)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId);
        if (supplier == null)
        {
            return ApiResponse<SupplierResponseDto>.FailureResult("Supplier tidak ditemukan.");
        }

        supplier.CompanyName = request.CompanyName;
        supplier.ContactName = request.ContactName;
        supplier.ContactTitle = request.ContactTitle;
        supplier.Address = request.Address;
        supplier.City = request.City;
        supplier.Region = request.Region;
        supplier.PostalCode = request.PostalCode;
        supplier.Country = request.Country;
        supplier.Phone = request.Phone;
        supplier.Fax = request.Fax;
        supplier.HomePage = request.HomePage;

        await _supplierRepository.UpdateAsync(supplier);
        await _supplierRepository.SaveChangesAsync();

        return ApiResponse<SupplierResponseDto>.SuccessResult(MapToResponseDto(supplier), "Supplier berhasil diperbarui.");
    }

    public async Task<ApiResponse<bool>> DeleteSupplierAsync(int id)
    {
        var supplier = await _supplierRepository.GetByIdWithProductsAsync(id);
        if (supplier == null)
        {
            return ApiResponse<bool>.FailureResult("Supplier tidak ditemukan.");
        }

        if (supplier.Products.Count != 0)
        {
            return ApiResponse<bool>.FailureResult("Supplier tidak dapat dihapus karena masih terikat dengan beberapa produk.");
        }

        await _supplierRepository.DeleteAsync(supplier);
        await _supplierRepository.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResult(true, "Supplier berhasil dihapus.");
    }

    private static SupplierResponseDto MapToResponseDto(Supplier supplier) => new()
    {
        SupplierId = supplier.SupplierId,
        CompanyName = supplier.CompanyName,
        ContactName = supplier.ContactName,
        ContactTitle = supplier.ContactTitle,
        Address = supplier.Address,
        City = supplier.City,
        Region = supplier.Region,
        PostalCode = supplier.PostalCode,
        Country = supplier.Country,
        Phone = supplier.Phone,
        Fax = supplier.Fax,
        HomePage = supplier.HomePage
    };
}