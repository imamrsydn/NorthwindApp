namespace Northwind.ViewModel.UserAccount
{
    public class UserAccountListDto
    {
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int CountLog { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}