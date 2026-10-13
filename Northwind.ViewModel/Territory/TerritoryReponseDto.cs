namespace Northwind.ViewModel.Territory
{
    public class TerritoryResponseDto
    {
        public string TerritoryId { get; set; } = string.Empty;
        public string TerritoryDescription { get; set; } = string.Empty;
        public int RegionId { get; set; }
        public string RegionDescription { get; set; } = string.Empty;
    }
}
