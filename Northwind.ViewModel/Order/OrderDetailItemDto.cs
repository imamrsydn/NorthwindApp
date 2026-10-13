namespace Northwind.ViewModel.Order
{
    public class OrderDetailItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; }
        public float Discount { get; set; }
        public decimal SubTotal => (UnitPrice * Quantity) * (decimal)(1 - Discount);
    }

    public class OrderDetailDto : OrderResponseDto
    {
        public List<OrderDetailItemDto> Items { get; set; } = new();
    }
}