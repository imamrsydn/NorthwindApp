using System;
using System.Collections.Generic;
using System.Text;

namespace Northwind.ViewModel.Report
{
    public class ProductStatusReportDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public short UnitsInStock { get; set; }
        public short ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
        public string Status { get; set; } = string.Empty; // LowStock, TopSelling, Discontinued, Continued
    }
}
