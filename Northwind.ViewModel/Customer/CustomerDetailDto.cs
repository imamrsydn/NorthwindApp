namespace Northwind.ViewModel.Customer
{
    public class CustomerDetailDto
    {
        // 1. Company & Contact Info
        public CustomerResponseDto CompanyInfo { get; set; } = new();

        // 2. Customer Order History
        public List<CustomerOrderSummaryDto> Orders { get; set; } = [];

        // 3. Customer Statistic
        public CustomerStatisticDto Statistic { get; set; } = new();
    }

    public class CustomerOrderSummaryDto
    {
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
    }

    public class CustomerStatisticDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal AverageMonthlySpent3Months { get; set; }
        public int OrdersLast3Months { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public string Classification { get; set; } = "Never Purchased";
    }
}