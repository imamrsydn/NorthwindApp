namespace Northwind.ViewModel.Supplier
{
    public class SupplierFilterDto : PagedFilterDto
    {
        public string? CompanyName { get; set; }
        public string? Country { get; set; }
    }
}