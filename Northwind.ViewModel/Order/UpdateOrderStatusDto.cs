using System.ComponentModel.DataAnnotations;

namespace Northwind.ViewModel.Order
{
    public class UpdateOrderStatusDto
    {
        [Required]
        public int OrderId { get; set; }

        // Jika diisi DateTime, status otomatis Shipped. Jika null, status Pending.
        public DateTime? ShippedDate { get; set; }
    }
}