namespace Northwind.ViewModel.Employee
{
    public class EmployeeFilterDto : PagedFilterDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Title { get; set; }
    }
}