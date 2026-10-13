using Northwind.ViewModel;
using Northwind.ViewModel.Auth;
using Northwind.ViewModel.Customer;

namespace Northwind.Provider.Interfaces;

public interface ICustomerProvider
{
    Task<ApiResponse<List<CustomerResponseDto>>> GetAllCustomersAsync(string? companyName, string? contactPerson, string? region);
    Task<ApiResponse<PagedResultDto<CustomerResponseDto>>> GetPagedCustomersAsync(CustomerFilterDto filter);
    Task<ApiResponse<CustomerDetailDto>> GetCustomerByIdAsync(string customerId);
    Task<ApiResponse<CustomerResponseDto>> CreateCustomerAsync(CreateCustomerDto request);
    Task<ApiResponse<CustomerResponseDto>> UpdateCustomerAsync(UpdateCustomerDto request);
    Task<ApiResponse<bool>> DeleteCustomerAsync(string customerId);
}