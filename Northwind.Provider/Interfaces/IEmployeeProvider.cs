using Northwind.ViewModel;
using Northwind.ViewModel.Auth;
using Northwind.ViewModel.Employee;

namespace Northwind.Provider.Interfaces
{
    public interface IEmployeeProvider
    {
        Task<ApiResponse<List<EmployeeResponseDto>>> GetAllEmployeesAsync(int? employeeId, string? fullName, string? territory);
        Task<ApiResponse<PagedResultDto<EmployeeResponseDto>>> GetPagedEmployeesAsync(EmployeeFilterDto filter);
        Task<ApiResponse<EmployeeDetailDto>> GetEmployeeByIdAsync(int id);
        Task<ApiResponse<EmployeeResponseDto>> CreateEmployeeAsync(CreateEmployeeDto request);
        Task<ApiResponse<EmployeeResponseDto>> UpdateEmployeeAsync(UpdateEmployeeDto request);
        Task<ApiResponse<bool>> DeleteEmployeeAsync(int id);
        Task<ApiResponse<bool>> AssignTerritoriesAsync(AssignTerritoriesDto request);
    }
}