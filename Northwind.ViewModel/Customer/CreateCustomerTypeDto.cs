using System.ComponentModel.DataAnnotations;

namespace Northwind.ViewModel.Customer
{
    public class CreateCustomerTypeDto
    {
        [Required]
        [MaxLength(10)]
        public string CustomerTypeId { get; set; } = string.Empty;

        public string? CustomerDesc { get; set; }
    }
}
