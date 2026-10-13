using Northwind.ViewModel.Auth;
using Northwind.ViewModel.Report;

namespace Northwind.Provider.Interfaces
{
    public interface IReportProvider
    {
        Task<ApiResponse<List<SalesReportDto>>> GetSalesByGroupAsync(string groupBy, int? year);
    }
}