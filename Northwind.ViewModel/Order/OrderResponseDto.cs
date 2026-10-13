namespace Northwind.ViewModel.Order
{
    public class OrderResponseDto
    {
        public int OrderId { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerCompanyName { get; set; }
        public int? EmployeeId { get; set; }
        public string? EmployeeFullName { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string Status => ShippedDate.HasValue ? "Shipped" : "Pending";
        public int? ShipVia { get; set; }
        public string? ShipperCompanyName { get; set; }
        public decimal? Freight { get; set; }
        public string? ShipName { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipRegion { get; set; }
        public string? ShipPostalCode { get; set; }
        public string? ShipCountry { get; set; }
        public decimal TotalAmount { get; set; }
    }
}