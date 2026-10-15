using Northwind.DbAccess.Models;
using Northwind.Provider.Interfaces;
using Northwind.Repository.Interfaces;
using Northwind.ViewModel;
using Northwind.ViewModel.Auth;
using Northwind.ViewModel.Employee;
using Northwind.ViewModel.Territory;

namespace Northwind.Provider.Implementations
{
    public class EmployeeProvider : IEmployeeProvider
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeProvider(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<ApiResponse<List<EmployeeResponseDto>>> GetAllEmployeesAsync(int? employeeId, string? fullName, string? territory)
        {
            var employees = await _employeeRepository.GetAllAsync(employeeId, fullName, territory);
            var response = employees.Select(MapToResponseDto).ToList();
            return ApiResponse<List<EmployeeResponseDto>>.SuccessResult(response);
        }

        public async Task<ApiResponse<PagedResultDto<EmployeeResponseDto>>> GetPagedEmployeesAsync(EmployeeFilterDto filter)
        {
            var (items, totalCount) = await _employeeRepository.GetPagedAsync(filter);
            var dtos = items.Select(MapToResponseDto).ToList();

            var result = new PagedResultDto<EmployeeResponseDto>(dtos, totalCount, filter.PageIndex, filter.PageSize);
            return ApiResponse<PagedResultDto<EmployeeResponseDto>>.SuccessResult(result);
        }

        public async Task<ApiResponse<EmployeeDetailDto>> GetEmployeeByIdAsync(int id)
        {
            var emp = await _employeeRepository.GetByIdWithTerritoriesAsync(id);
            if (emp == null)
                return ApiResponse<EmployeeDetailDto>.FailureResult("Employee tidak ditemukan.");

            var detail = new EmployeeDetailDto
            {
                EmployeeId = emp.EmployeeId,
                FirstName = emp.FirstName,
                LastName = emp.LastName,
                Title = emp.Title,
                TitleOfCourtesy = emp.TitleOfCourtesy,
                BirthDate = emp.BirthDate,
                HireDate = emp.HireDate,
                Address = emp.Address,
                City = emp.City,
                Region = emp.Region,
                PostalCode = emp.PostalCode,
                Country = emp.Country,
                HomePhone = emp.HomePhone,
                Extension = emp.Extension,
                Notes = emp.Notes,
                ReportsTo = emp.ReportsTo,
                ReportsToName = emp.ReportsToNavigation != null
                    ? $"{emp.ReportsToNavigation.FirstName} {emp.ReportsToNavigation.LastName}"
                    : null,
                PhotoPath = emp.PhotoPath,
                Territories = emp.Territories.Select(t => new TerritoryResponseDto
                {
                    TerritoryId = t.TerritoryId.Trim(),
                    TerritoryDescription = t.TerritoryDescription.Trim(),
                    RegionId = t.RegionId,
                    RegionDescription = t.Region.RegionDescription.Trim()
                }).ToList()
            };

            return ApiResponse<EmployeeDetailDto>.SuccessResult(detail);
        }

        public async Task<ApiResponse<EmployeeResponseDto>> CreateEmployeeAsync(CreateEmployeeDto request)
        {
            var employee = new Employee
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Title = request.Title,
                TitleOfCourtesy = request.TitleOfCourtesy,
                BirthDate = request.BirthDate,
                HireDate = request.HireDate,
                Address = request.Address,
                City = request.City,
                Region = request.Region,
                PostalCode = request.PostalCode,
                Country = request.Country,
                HomePhone = request.HomePhone,
                Extension = request.Extension,
                Notes = request.Notes,
                ReportsTo = request.ReportsTo,
                PhotoPath = request.PhotoPath
            };

            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.SaveChangesAsync();

            var created = await _employeeRepository.GetByIdAsync(employee.EmployeeId);
            return ApiResponse<EmployeeResponseDto>.SuccessResult(MapToResponseDto(created!), "Employee berhasil ditambahkan.");
        }

        public async Task<ApiResponse<EmployeeResponseDto>> UpdateEmployeeAsync(UpdateEmployeeDto request)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
            if (employee == null)
                return ApiResponse<EmployeeResponseDto>.FailureResult("Employee tidak ditemukan.");

            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.Title = request.Title;
            employee.TitleOfCourtesy = request.TitleOfCourtesy;
            employee.BirthDate = request.BirthDate;
            employee.HireDate = request.HireDate;
            employee.Address = request.Address;
            employee.City = request.City;
            employee.Region = request.Region;
            employee.PostalCode = request.PostalCode;
            employee.Country = request.Country;
            employee.HomePhone = request.HomePhone;
            employee.Extension = request.Extension;
            employee.Notes = request.Notes;
            employee.ReportsTo = request.ReportsTo;
            employee.PhotoPath = request.PhotoPath;

            await _employeeRepository.UpdateAsync(employee);
            await _employeeRepository.SaveChangesAsync();

            var updated = await _employeeRepository.GetByIdAsync(employee.EmployeeId);
            return ApiResponse<EmployeeResponseDto>.SuccessResult(MapToResponseDto(updated!), "Employee berhasil diperbarui.");
        }

        public async Task<ApiResponse<bool>> DeleteEmployeeAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return ApiResponse<bool>.FailureResult("Employee tidak ditemukan.");

            await _employeeRepository.DeleteAsync(employee);
            await _employeeRepository.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResult(true, "Employee berhasil dihapus.");
        }

        public async Task<ApiResponse<bool>> AssignTerritoriesAsync(AssignTerritoriesDto request)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
            if (employee == null)
                return ApiResponse<bool>.FailureResult("Employee tidak ditemukan.");

            await _employeeRepository.AssignTerritoriesAsync(request.EmployeeId, request.TerritoryIds);
            await _employeeRepository.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResult(true, "Mapping Territory untuk Employee berhasil diperbarui.");
        }

        private static EmployeeResponseDto MapToResponseDto(Employee emp) => new()
        {
            EmployeeId = emp.EmployeeId,
            FirstName = emp.FirstName,
            LastName = emp.LastName,
            Title = emp.Title,
            TitleOfCourtesy = emp.TitleOfCourtesy,
            BirthDate = emp.BirthDate,
            HireDate = emp.HireDate,
            Address = emp.Address,
            City = emp.City,
            Region = emp.Region,
            PostalCode = emp.PostalCode,
            Country = emp.Country,
            HomePhone = emp.HomePhone,
            Extension = emp.Extension,
            Notes = emp.Notes,
            ReportsTo = emp.ReportsTo,
            ReportsToName = emp.ReportsToNavigation != null
                ? $"{emp.ReportsToNavigation.FirstName} {emp.ReportsToNavigation.LastName}"
                : null,
            PhotoPath = emp.PhotoPath,
            TotalTerritory = emp.Territories.Count
        };
    }
}