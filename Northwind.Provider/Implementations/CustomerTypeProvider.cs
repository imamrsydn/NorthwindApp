using Northwind.DbAccess.Models;
using Northwind.Provider.Interfaces;
using Northwind.Repository.Interfaces;
using Northwind.ViewModel.Auth;
using Northwind.ViewModel.Customer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Northwind.Provider.Implementations
{
    public class CustomerTypeProvider : ICustomerTypeProvider
    {
        private readonly ICustomerTypeRepository _repo;

        public CustomerTypeProvider(ICustomerTypeRepository repo) { _repo = repo; }

        public async Task<ApiResponse<List<CustomerTypeResponseDto>>> GetAllAsync()
        {
            var data = await _repo.GetAllAsync();
            var res = data.Select(x => new CustomerTypeResponseDto { CustomerTypeId = x.CustomerTypeId.Trim(), CustomerDesc = x.CustomerDesc }).ToList();
            return ApiResponse<List<CustomerTypeResponseDto>>.SuccessResult(res);
        }

        public async Task<ApiResponse<CustomerTypeResponseDto>> CreateAsync(CreateCustomerTypeDto req)
        {
            var entity = new CustomerDemographic { CustomerTypeId = req.CustomerTypeId, CustomerDesc = req.CustomerDesc };
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return ApiResponse<CustomerTypeResponseDto>.SuccessResult(new CustomerTypeResponseDto { CustomerTypeId = entity.CustomerTypeId, CustomerDesc = entity.CustomerDesc }, "Kategori tipe customer berhasil dibuat.");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(string id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return ApiResponse<bool>.FailureResult("Data tidak ditemukan.");
            await _repo.DeleteAsync(entity);
            await _repo.SaveChangesAsync();
            return ApiResponse<bool>.SuccessResult(true, "Berhasil dihapus.");
        }
    }
}
