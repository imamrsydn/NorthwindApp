using System;
using System.Collections.Generic;
using System.Text;

namespace Northwind.ViewModel.Dashboard
{
    public class TopCustomerDto
    {
        public string CustomerId { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public int RepeatOrderCount { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
