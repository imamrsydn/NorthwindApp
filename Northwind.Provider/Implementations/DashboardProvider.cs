using Northwind.Provider.Interfaces;
using Northwind.Repository.Interfaces;
using Northwind.ViewModel.Auth;
using Northwind.ViewModel.Dashboard;

namespace Northwind.Provider.Implementations
{
    public class DashboardProvider : IDashboardProvider
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardProvider(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<ApiResponse<DashboardSummaryDto>> GetDashboardSummaryAsync()
        {
            var topProdData = await _dashboardRepository.GetTopProductsAsync(5);
            var topCustData = await _dashboardRepository.GetTopCustomersAsync(5);
            var overdueData = await _dashboardRepository.GetOverdueOrdersAsync();

            var today = DateTime.UtcNow;

            var summary = new DashboardSummaryDto
            {
                TotalProducts = await _dashboardRepository.GetTotalProductsAsync(),
                ActiveProducts = await _dashboardRepository.GetActiveProductsAsync(),
                TotalCustomers = await _dashboardRepository.GetTotalCustomersAsync(),
                TotalEmployees = await _dashboardRepository.GetTotalEmployeesAsync(),
                TotalOrders = await _dashboardRepository.GetTotalOrdersAsync(),
                PendingOrders = await _dashboardRepository.GetPendingOrdersAsync(),
                ShippedOrders = await _dashboardRepository.GetShippedOrdersAsync(),
                TotalSales = Math.Round(await _dashboardRepository.GetTotalSalesAsync(), 2),
                TopProducts = topProdData.Select(x => new TopProductDto
                {
                    ProductId = x.Item1,
                    ProductName = x.Item2,
                    QuantitySold = x.Item3,
                    TotalRevenue = Math.Round(x.Item4, 2)
                }).ToList(),
                TopCustomers = topCustData.Select(x => new TopCustomerDto
                {
                    CustomerId = x.Item1,
                    CompanyName = x.Item2,
                    RepeatOrderCount = x.Item3,
                    TotalSpent = Math.Round(x.Item4, 2)
                }).ToList(),
                OverdueOrders = overdueData.Select(o => new OverdueOrderDto
                {
                    OrderId = o.OrderId,
                    CustomerName = o.Customer?.CompanyName ?? string.Empty,
                    OrderDate = o.OrderDate,
                    RequiredDate = o.RequiredDate,
                    DaysOverdue = o.RequiredDate.HasValue ? (today - o.RequiredDate.Value).Days : 0
                }).ToList()
            };

            return ApiResponse<DashboardSummaryDto>.SuccessResult(summary);
        }
    }
}