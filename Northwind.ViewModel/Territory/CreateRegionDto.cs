using System.ComponentModel.DataAnnotations;

namespace Northwind.ViewModel.Territory
{
    public class CreateRegionDto
    {
        [Required]
        public int RegionId { get; set; }

        [Required]
        [MaxLength(50)]
        public string RegionDescription { get; set; } = string.Empty;
    }
}
