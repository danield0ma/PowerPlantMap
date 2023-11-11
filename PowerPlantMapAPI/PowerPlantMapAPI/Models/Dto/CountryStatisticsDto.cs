namespace PowerPlantMapAPI.Models.DTO;

public class CountryStatisticsDto
{
    public string? CountryId { get; set; }
    public string? CountryName { get; set; }
    public double ImportedEnergy { get; set; }
    public double ExportedEnergy { get; set; }
}