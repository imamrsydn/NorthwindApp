using System.ComponentModel.DataAnnotations;

namespace Northwind.ViewModel.Shipper;

public class UpdateShipperDto : CreateShipperDto
{
    [Required(ErrorMessage = "ShipperId wajib diisi saat update.")]
    public int ShipperId { get; set; }
}