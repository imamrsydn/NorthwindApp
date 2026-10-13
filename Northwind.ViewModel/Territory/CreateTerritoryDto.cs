using System.ComponentModel.DataAnnotations;

namespace Northwind.ViewModel.Territory
{
    public class CreateTerritoryDto
    {
        [Required]
        [MaxLength(20)]
        public string TerritoryId { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string TerritoryDescription { get; set; } = string.Empty;

        [Required]
        public int RegionId { get; set; }
    }
}
