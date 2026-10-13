using Northwind.Provider.Interfaces;
using Northwind.Repository.Interfaces;
using Northwind.ViewModel.Auth;
using Northwind.ViewModel.Report;

namespace Northwind.Provider.Implementations
{
    public class ReportProvider : IReportProvider
    {
        private readonly IReportRepository _repo;

        public ReportProvider(IReportRepository repo) { _repo = repo; }

        public async Task<ApiResponse<List<SalesReportDto>>> GetSalesByGroupAsync(string groupBy, int? year)
        {
            List<Tuple<string, int, decimal>> data = groupBy.ToLower() switch
            {
                "bymonth" => await _repo.GetSalesByMonthAsync(year ?? DateTime.UtcNow.Year),
                "byproduct" => await _repo.GetSalesByProductAsync(),
                "bycategory" => await _repo.GetSalesByCategoryAsync(),
                "bycustomer" => await _repo.GetSalesByCustomerAsync(),
                "byemployee" => await _repo.GetSalesByEmployeeAsync(),
                _ => await _repo.GetSalesByMonthAsync(DateTime.UtcNow.Year)
            };

            var res = data.Select(x => new SalesReportDto
            {
                GroupKey = x.Item1,
                TotalOrders = x.Item2,
                TotalSalesAmount = Math.Round(x.Item3, 2)
            }).ToList();

            return ApiResponse<List<SalesReportDto>>.SuccessResult(res);
        }
    }
}