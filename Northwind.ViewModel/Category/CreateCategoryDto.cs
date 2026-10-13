using System.ComponentModel.DataAnnotations;

namespace Northwind.ViewModel.Category
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Nama kategori wajib diisi.")]
        [MaxLength(15, ErrorMessage = "Nama kategori maksimal 15 karakter.")]
        public string CategoryName { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}