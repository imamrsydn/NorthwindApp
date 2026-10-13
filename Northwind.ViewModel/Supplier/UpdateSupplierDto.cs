using System.ComponentModel.DataAnnotations;

namespace Northwind.ViewModel.Supplier
{
    public class UpdateSupplierDto : CreateSupplierDto
    {
        [Required(ErrorMessage = "SupplierId wajib diisi saat update.")]
        public int SupplierId { get; set; }
    }
}