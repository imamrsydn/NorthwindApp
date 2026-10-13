namespace Northwind.ViewModel.Supplier
{
    public class SupplierDetailDto : SupplierResponseDto
    {
        public List<SupplierProductDto> Products { get; set; } = [];
    }

    public class SupplierProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
        public bool Discontinued { get; set; }
    }
}