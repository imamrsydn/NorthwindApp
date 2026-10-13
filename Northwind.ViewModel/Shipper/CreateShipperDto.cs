using System.ComponentModel.DataAnnotations;

namespace Northwind.ViewModel.Shipper
{
    public class CreateShipperDto
    {
        [Required(ErrorMessage = "Nama ekspedisi wajib diisi.")]
        [MaxLength(40, ErrorMessage = "Maksimal 40 karakter.")]
        public string CompanyName { get; set; } = string.Empty;

        [MaxLength(24, ErrorMessage = "Nomor telepon maksimal 24 karakter.")]
        public string? Phone { get; set; }
    }
}