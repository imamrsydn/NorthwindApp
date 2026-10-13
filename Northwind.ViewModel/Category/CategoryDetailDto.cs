namespace Northwind.ViewModel.Category
{
    public class CategoryDetailDto : CategoryResponseDto
    {
        public List<CategoryProductDto> Products { get; set; } = [];
    }

    public class CategoryProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
        public bool Discontinued { get; set; }
    }
}