using System.ComponentModel.DataAnnotations;

namespace Northwind.ViewModel.Category
{
    public class UpdateCategoryDto : CreateCategoryDto
    {
        [Required(ErrorMessage = "CategoryId wajib diisi saat update.")]
        public int CategoryId { get; set; }
    }
}