namespace Northwind.ViewModel.Order
{
    public class OrderFilterDto : PagedFilterDto
    {
        public string? CustomerId { get; set; }
        public int? SupplierId { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}