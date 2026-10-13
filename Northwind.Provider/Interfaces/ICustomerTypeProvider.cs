using Northwind.ViewModel.Auth;
using Northwind.ViewModel.Customer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Northwind.Provider.Interfaces
{
    public interface ICustomerTypeProvider
    {
        Task<ApiResponse<List<CustomerTypeResponseDto>>> GetAllAsync();
        Task<ApiResponse<CustomerTypeResponseDto>> CreateAsync(CreateCustomerTypeDto req);
        Task<ApiResponse<bool>> DeleteAsync(string id);
    }
}
