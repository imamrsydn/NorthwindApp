using System;
using System.Collections.Generic;
using System.Text;

namespace Northwind.ViewModel.UserAccount
{
    public class UserAccountDetailDto : UserAccountListDto
    {
        public int? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
