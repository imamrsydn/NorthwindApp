namespace Northwind.ViewModel.Customer
{
    public class CustomerFilterDto : PagedFilterDto
    {
        public string? CompanyName { get; set; }
        public string? ContactName { get; set; }
        public string? Country { get; set; }
        public string? CustomerType { get; set; }
    }
}