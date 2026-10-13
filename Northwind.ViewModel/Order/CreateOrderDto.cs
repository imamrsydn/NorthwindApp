using System.ComponentModel.DataAnnotations;

namespace Northwind.ViewModel.Order
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "CustomerId wajib diisi.")]
        [StringLength(5, MinimumLength = 5)]
        public string CustomerId { get; set; } = string.Empty;

        public int? EmployeeId { get; set; }
        public DateTime? RequiredDate { get; set; }
        public int? ShipVia { get; set; }

        [Range(0, 999999.99)]
        public decimal? Freight { get; set; }

        [MaxLength(40)]
        public string? ShipName { get; set; }

        [MaxLength(60)]
        public string? ShipAddress { get; set; }

        [MaxLength(15)]
        public string? ShipCity { get; set; }

        [MaxLength(15)]
        public string? ShipRegion { get; set; }

        [MaxLength(10)]
        public string? ShipPostalCode { get; set; }

        [MaxLength(15)]
        public string? ShipCountry { get; set; }

        [Required(ErrorMessage = "Order item tidak boleh kosong.")]
        [MinLength(1, ErrorMessage = "Minimal harus membeli 1 item produk.")]
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }

    public class CreateOrderItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 32767, ErrorMessage = "Quantity minimal 1.")]
        public short Quantity { get; set; }

        [Range(0, 1.0, ErrorMessage = "Diskon bernilai antara 0.0 sampai 1.0 (0% - 100%).")]
        public float Discount { get; set; } = 0;
    }
}