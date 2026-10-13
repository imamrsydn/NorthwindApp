using System.ComponentModel.DataAnnotations;

namespace Northwind.ViewModel.Employee
{
    public class AssignTerritoriesDto
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "TerritoryIds tidak boleh kosong.")]
        public List<string> TerritoryIds { get; set; } = new();
    }
}