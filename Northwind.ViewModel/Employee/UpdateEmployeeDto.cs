using System.ComponentModel.DataAnnotations;

namespace Northwind.ViewModel.Employee;

public class UpdateEmployeeDto : CreateEmployeeDto
{
    [Required(ErrorMessage = "EmployeeId wajib diisi saat update.")]
    public int EmployeeId { get; set; }
}