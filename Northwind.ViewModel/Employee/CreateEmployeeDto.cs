using System.ComponentModel.DataAnnotations;

namespace Northwind.ViewModel.Employee
{
    public class CreateEmployeeDto
    {
        [Required(ErrorMessage = "First Name wajib diisi.")]
        [MaxLength(10)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name wajib diisi.")]
        [MaxLength(20)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? Title { get; set; }

        [MaxLength(25)]
        public string? TitleOfCourtesy { get; set; }

        public DateTime? BirthDate { get; set; }
        public DateTime? HireDate { get; set; }

        [MaxLength(60)]
        public string? Address { get; set; }

        [MaxLength(15)]
        public string? City { get; set; }

        [MaxLength(15)]
        public string? Region { get; set; }

        [MaxLength(10)]
        public string? PostalCode { get; set; }

        [MaxLength(15)]
        public string? Country { get; set; }

        [MaxLength(24)]
        public string? HomePhone { get; set; }

        [MaxLength(4)]
        public string? Extension { get; set; }

        public string? Notes { get; set; }
        public int? ReportsTo { get; set; }

        [MaxLength(255)]
        public string? PhotoPath { get; set; }
    }
}