using System.ComponentModel.DataAnnotations;

namespace Northwind.ViewModel.Customer
{
    public class CreateCustomerDto
    {
        [Required(ErrorMessage = "CustomerId (5 Karakter) wajib diisi.")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "CustomerId harus tepat 5 karakter.")]
        public string CustomerId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nama Perusahaan wajib diisi.")]
        [MaxLength(40)]
        public string CompanyName { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? ContactName { get; set; }

        [MaxLength(30)]
        public string? ContactTitle { get; set; }

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
        public string? Phone { get; set; }

        [MaxLength(24)]
        public string? Fax { get; set; }
    }
}