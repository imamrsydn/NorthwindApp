namespace Northwind.ViewModel.Auth
{
    public class UserProfileDto
    {
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Role { get; set; } = string.Empty;
        public int? EmployeeId { get; set; }
        public string? CustomerId { get; set; }
        public bool IsActive { get; set; }
        public int CountLog { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}