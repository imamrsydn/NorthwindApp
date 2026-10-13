using System.ComponentModel.DataAnnotations;

namespace Northwind.ViewModel.Product
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Nama produk wajib diisi.")]
        [MaxLength(40, ErrorMessage = "Nama produk maksimal 40 karakter.")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Supplier wajib dipilih.")]
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Category wajib dipilih.")]
        public int CategoryId { get; set; }

        [MaxLength(20)]
        public string? QuantityPerUnit { get; set; }

        [Range(0, 9999999.99, ErrorMessage = "Harga unit harus positif.")]
        public decimal? UnitPrice { get; set; }

        [Range(0, 32767, ErrorMessage = "Stok harus bernilai positif.")]
        public short? UnitsInStock { get; set; }

        [Range(0, 32767)]
        public short? UnitsOnOrder { get; set; }

        [Range(0, 32767)]
        public short? ReorderLevel { get; set; }

        public bool Discontinued { get; set; } = false;
    }
}