using System.ComponentModel.DataAnnotations;

namespace Northwind.ViewModel.Customer
{
    public class CustomerTypeResponseDto
    {
        public string CustomerTypeId { get; set; } = string.Empty;
        public string? CustomerDesc { get; set; }
    }
}