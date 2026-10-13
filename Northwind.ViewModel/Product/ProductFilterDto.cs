namespace Northwind.ViewModel.Product
{
    public class ProductFilterDto : PagedFilterDto
    {
        public int? CategoryId { get; set; }
        public int? SupplierId { get; set; }
        public bool? Discontinued { get; set; }
        public bool? ReadyToOrder { get; set; }
    }
}