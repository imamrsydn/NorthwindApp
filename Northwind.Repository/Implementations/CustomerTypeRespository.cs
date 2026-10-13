using Microsoft.EntityFrameworkCore;
using Northwind.DbAccess.Models;
using Northwind.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Northwind.Repository.Implementations
{
    public class CustomerTypeRepository : ICustomerTypeRepository
    {
        private readonly Northwind3Context _context;

        public CustomerTypeRepository(Northwind3Context context) { _context = context; }
        public async Task<List<CustomerDemographic>> GetAllAsync() => await _context.CustomerDemographics.ToListAsync();
        public async Task<CustomerDemographic?> GetByIdAsync(string id) => await _context.CustomerDemographics.FindAsync(id);
        public async Task AddAsync(CustomerDemographic entity) => await _context.CustomerDemographics.AddAsync(entity);
        public async Task DeleteAsync(CustomerDemographic entity) { _context.CustomerDemographics.Remove(entity); await Task.CompletedTask; }
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
