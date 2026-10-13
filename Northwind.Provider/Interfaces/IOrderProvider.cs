using Northwind.ViewModel;
using Northwind.ViewModel.Auth;
using Northwind.ViewModel.Order;

namespace Northwind.Provider.Interfaces
{
    public interface IOrderProvider
    {
        Task<ApiResponse<List<OrderResponseDto>>> GetAllOrdersAsync(string? customerId, int? supplierId, int? employeeId, DateTime? startDate, DateTime? endDate, string? userRole, string? currentCustomerId, int? currentEmployeeId);
        Task<ApiResponse<PagedResultDto<OrderResponseDto>>> GetPagedOrdersAsync(OrderFilterDto filter, string? userRole, string? currentCustomerId, int? currentEmployeeId);
        Task<ApiResponse<OrderDetailDto>> GetOrderByIdAsync(int id, string? userRole, string? currentCustomerId, int? currentEmployeeId);
        Task<ApiResponse<OrderDetailDto>> CreateOrderAsync(CreateOrderDto request);
        Task<ApiResponse<bool>> UpdateOrderStatusAsync(UpdateOrderStatusDto request);
        Task<ApiResponse<bool>> DeleteOrderAsync(int id);
    }
}