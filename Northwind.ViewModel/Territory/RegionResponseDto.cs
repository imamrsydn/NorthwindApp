namespace Northwind.ViewModel.Territory
{
    public class RegionResponseDto
    {
        public int RegionId { get; set; }
        public string RegionDescription { get; set; } = string.Empty;
        public List<TerritoryResponseDto> ListTerritory { get; set; } = [];
        public int TotalTerritory => ListTerritory.Count;
    }
}