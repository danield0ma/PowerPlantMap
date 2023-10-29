namespace PowerPlantMapAPI.Data.Dto;

public class CreatePowerPlantDto
{
    public string? PowerPlantId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? OperatorCompany { get; set; }
    public string? Webpage { get; set; }
    public string? Image { get; set; }
    public float Longitude { get; set; }
    public float Latitude { get; set; }
    public string? Color { get; set; }
    public string? Address { get; set; }
    public bool IsCountry { get; set; }
    public List<BlocDataDto>? Blocs { get; set; }
}