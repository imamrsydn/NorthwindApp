using Northwind.ViewModel.Territory;
using Northwind.ViewModel.Order;

using Northwind.ViewModel.UserAccount;

namespace Northwind.ViewModel.Employee
{
    public class EmployeeDetailDto : EmployeeResponseDto
    {
        public List<TerritoryResponseDto>? Territories { get; set; } = [];
        public List<string>? Members { get; set; } = [];

        public List<OrderResponseDto>? Orders { get; set; } = [];

        public UserAccountDetailDto? UserAccounts { get; set; }
    }
}