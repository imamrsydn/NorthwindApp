using Northwind.DbAccess.Models;
using Northwind.Provider.Interfaces;
using Northwind.Repository.Interfaces;
using Northwind.ViewModel;
using Northwind.ViewModel.Auth;
using Northwind.ViewModel.Customer;

namespace Northwind.Provider.Implementations
{
    public class CustomerProvider : ICustomerProvider
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerProvider(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<ApiResponse<List<CustomerResponseDto>>> GetAllCustomersAsync(string? companyName, string? contactPerson, string? region)
        {
            var customers = await _customerRepository.GetAllAsync(companyName, contactPerson, region);
            var top10Ids = await _customerRepository.GetTop10CustomerIdsInLast3MonthsAsync();

            var response = customers.Select(c =>
            {
                var dto = MapToResponseDto(c);
                dto.CustomerCategory = CalculateCategory(c, top10Ids);
                return dto;
            }).ToList();

            return ApiResponse<List<CustomerResponseDto>>.SuccessResult(response);
        }

        public async Task<ApiResponse<PagedResultDto<CustomerResponseDto>>> GetPagedCustomersAsync(CustomerFilterDto filter)
        {
            var (items, totalCount) = await _customerRepository.GetPagedAsync(filter);
            var dtos = items.Select(MapToResponseDto).ToList();

            var result = new PagedResultDto<CustomerResponseDto>(dtos, totalCount, filter.PageIndex, filter.PageSize);
            return ApiResponse<PagedResultDto<CustomerResponseDto>>.SuccessResult(result);
        }

        public async Task<ApiResponse<CustomerDetailDto>> GetCustomerByIdAsync(string customerId)
        {
            var c = await _customerRepository.GetByIdWithOrdersAsync(customerId);
            if (c == null)
                return ApiResponse<CustomerDetailDto>.FailureResult("Customer tidak ditemukan.");

            var top10Ids = await _customerRepository.GetTop10CustomerIdsInLast3MonthsAsync();

            var ordersSummary = c.Orders.Select(o => new CustomerOrderSummaryDto
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                ShippedDate = o.ShippedDate,
                TotalAmount = o.OrderDetails.Sum(od => od.UnitPrice * od.Quantity * (decimal)(1 - od.Discount)),
                TotalItems = o.OrderDetails.Sum(od => od.Quantity)
            }).OrderByDescending(o => o.OrderDate).ToList();

            var last3Months = DateTime.UtcNow.AddMonths(-3);
            var ordersInLast3Months = c.Orders.Where(o => o.OrderDate >= last3Months).ToList();

            decimal spent3Months = ordersInLast3Months
                .SelectMany(o => o.OrderDetails)
                .Sum(od => od.UnitPrice * od.Quantity * (decimal)(1 - od.Discount));

            decimal avgMonthlySpent = spent3Months / 3;

            var category = CalculateCategory(c, top10Ids);

            var detail = new CustomerDetailDto
            {
                CompanyInfo = MapToResponseDto(c),
                Orders = ordersSummary,
                Statistic = new CustomerStatisticDto
                {
                    TotalOrders = c.Orders.Count,
                    TotalSpent = ordersSummary.Sum(o => o.TotalAmount),
                    OrdersLast3Months = ordersInLast3Months.Count,
                    AverageMonthlySpent3Months = Math.Round(avgMonthlySpent, 2),
                    LastOrderDate = c.Orders.Max(o => o.OrderDate),
                    Classification = category
                }
            };

            detail.CompanyInfo.CustomerCategory = category;

            return ApiResponse<CustomerDetailDto>.SuccessResult(detail);
        }

        public async Task<ApiResponse<CustomerResponseDto>> CreateCustomerAsync(CreateCustomerDto request)
        {
            var existing = await _customerRepository.GetByIdAsync(request.CustomerId);
            if (existing != null)
                return ApiResponse<CustomerResponseDto>.FailureResult("CustomerId sudah digunakan.");

            var customer = new Customer
            {
                CustomerId = request.CustomerId.ToUpper(),
                CompanyName = request.CompanyName,
                ContactName = request.ContactName,
                ContactTitle = request.ContactTitle,
                Address = request.Address,
                City = request.City,
                Region = request.Region,
                PostalCode = request.PostalCode,
                Country = request.Country,
                Phone = request.Phone,
                Fax = request.Fax
            };

            await _customerRepository.AddAsync(customer);
            await _customerRepository.SaveChangesAsync();

            return ApiResponse<CustomerResponseDto>.SuccessResult(MapToResponseDto(customer), "Customer berhasil ditambahkan.");
        }

        public async Task<ApiResponse<CustomerResponseDto>> UpdateCustomerAsync(UpdateCustomerDto request)
        {
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
            if (customer == null)
                return ApiResponse<CustomerResponseDto>.FailureResult("Customer tidak ditemukan.");

            customer.CompanyName = request.CompanyName;
            customer.ContactName = request.ContactName;
            customer.ContactTitle = request.ContactTitle;
            customer.Address = request.Address;
            customer.City = request.City;
            customer.Region = request.Region;
            customer.PostalCode = request.PostalCode;
            customer.Country = request.Country;
            customer.Phone = request.Phone;
            customer.Fax = request.Fax;

            await _customerRepository.UpdateAsync(customer);
            await _customerRepository.SaveChangesAsync();

            return ApiResponse<CustomerResponseDto>.SuccessResult(MapToResponseDto(customer), "Customer berhasil diperbarui.");
        }

        public async Task<ApiResponse<bool>> DeleteCustomerAsync(string customerId)
        {
            var customer = await _customerRepository.GetByIdWithOrdersAsync(customerId);
            if (customer == null)
                return ApiResponse<bool>.FailureResult("Customer tidak ditemukan.");

            if (customer.Orders.Count != 0)
                return ApiResponse<bool>.FailureResult("Customer tidak bisa dihapus karena memiliki riwayat transaksi.");

            await _customerRepository.DeleteAsync(customer);
            await _customerRepository.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResult(true, "Customer berhasil dihapus.");
        }

        #region Helper Method Logic Klasifikasi Customer
        private static string CalculateCategory(Customer c, List<string> top10IdsIn3Months)
        {
            if (c.Orders == null || c.Orders.Count == 0)
                return "Never Purchased";

            var lastOrderDate = c.Orders.Max(o => o.OrderDate);
            var threeMonthsAgo = DateTime.UtcNow.AddMonths(-3);

            if (lastOrderDate == null || lastOrderDate <= threeMonthsAgo)
                return "InActive";

            var ordersIn3Months = c.Orders.Where(o => o.OrderDate >= threeMonthsAgo).ToList();
            decimal totalSpent3Months = ordersIn3Months
                .SelectMany(o => o.OrderDetails)
                .Sum(od => od.UnitPrice * od.Quantity * (decimal)(1 - od.Discount));

            decimal avgMonthly = totalSpent3Months / 3;

            // Rule VIP: Rata-rata Total Transaksi >= $2000/bulan ATAU Top 10 customer
            if (avgMonthly >= 2000 || top10IdsIn3Months.Contains(c.CustomerId))
                return "VIP";

            // Rule Distributor: Min 2 transaksi dalam 3 bulan terakhir & avg/bulan antara $500 - $1000
            if (ordersIn3Months.Count >= 2 && avgMonthly >= 500 && avgMonthly <= 1000)
                return "Distributor";

            return "Regular";
        }

        private static CustomerResponseDto MapToResponseDto(Customer c) => new()
        {
            CustomerId = c.CustomerId,
            CompanyName = c.CompanyName,
            ContactName = c.ContactName,
            ContactTitle = c.ContactTitle,
            Address = c.Address,
            City = c.City,
            Region = c.Region,
            PostalCode = c.PostalCode,
            Country = c.Country,
            Phone = c.Phone,
            Fax = c.Fax
        };
        #endregion
    }
}