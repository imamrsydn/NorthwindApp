namespace Northwind.ViewModel.Dashboard
{
    public class DashboardSummaryDto
    {
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ShippedOrders { get; set; }
        public decimal TotalSales { get; set; }

        public List<TopProductDto> TopProducts { get; set; } = new();
        public List<TopCustomerDto> TopCustomers { get; set; } = new();
        public List<OverdueOrderDto> OverdueOrders { get; set; } = new();
    }
}