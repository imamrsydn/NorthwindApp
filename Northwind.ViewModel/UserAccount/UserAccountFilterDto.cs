namespace Northwind.ViewModel.UserAccount
{
    public class UserAccountFilterDto : PagedFilterDto
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public bool? IsActive { get; set; }
    }
}