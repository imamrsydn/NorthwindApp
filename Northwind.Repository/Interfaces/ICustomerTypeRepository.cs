using Northwind.DbAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Northwind.Repository.Interfaces
{
    public interface ICustomerTypeRepository
    {
        Task<List<CustomerDemographic>> GetAllAsync();
        Task<CustomerDemographic?> GetByIdAsync(string id);
        Task AddAsync(CustomerDemographic entity);
        Task DeleteAsync(CustomerDemographic entity);
        Task SaveChangesAsync();
    }
}
