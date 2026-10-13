using Northwind.ViewModel.Auth;
using Northwind.ViewModel.Dashboard;

namespace Northwind.Provider.Interfaces
{
    public interface IDashboardProvider
    {
        Task<ApiResponse<DashboardSummaryDto>> GetDashboardSummaryAsync();
    }
}