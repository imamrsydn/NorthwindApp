using Northwind.ViewModel.Territory;

namespace Northwind.ViewModel.Employee
{
    public class EmployeeDetailDto : EmployeeResponseDto
    {
        public List<TerritoryResponseDto> Territories { get; set; } = [];
    }
}