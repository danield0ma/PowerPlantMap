namespace PowerPlantMapAPI.Data.Dto;

public class CountryStatisticsDto
{
    public string? CountryId { get; set; }
    public string? CountryName { get; set; }
    public string? Image { get; set; }
    public double ImportedEnergy { get; set; }
    public double ExportedEnergy { get; set; }
}