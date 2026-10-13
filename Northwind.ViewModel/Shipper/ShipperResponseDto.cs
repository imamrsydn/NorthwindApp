namespace Northwind.ViewModel.Shipper
{
    public class ShipperResponseDto
    {
        public int ShipperId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? Phone { get; set; }
    }
}