using System;
using System.Collections.Generic;
using System.Text;

namespace Northwind.ViewModel.Dashboard
{
    public class OverdueOrderDto
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public int DaysOverdue { get; set; }
    }
}
