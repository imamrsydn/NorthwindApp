using System.ComponentModel.DataAnnotations;

namespace Northwind.ViewModel.Product;

public class UpdateProductDto : CreateProductDto
{
    [Required(ErrorMessage = "ProductId wajib diisi saat update.")]
    public int ProductId { get; set; }
}