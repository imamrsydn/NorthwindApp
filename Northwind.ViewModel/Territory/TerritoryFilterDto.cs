namespace Northwind.ViewModel.Territory
{
    public class TerritoryFilterDto : PagedFilterDto
    {
        public int? RegionId { get; set; }
    }
}