namespace Northwind.ViewModel.Report
{
    public class SalesReportDto
    {
        public string GroupKey { get; set; } = string.Empty; // Nama Bulan/Produk/Kategori/Customer/Employee
        public int TotalOrders { get; set; }
        public decimal TotalSalesAmount { get; set; }
    }
}