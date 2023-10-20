using PowerPlantMapAPI.Models.DTO;

namespace PowerPlantMapAPI.Models;

public class PowerPlantDetailsModel
{
    public string? PowerPlantId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? OperatorCompany { get; set; }
    public string? Webpage { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public int CurrentPower { get; set; }
    public int MaxPower { get; set; }
    public DateTime DataStart { get; set; }
    public DateTime DataEnd { get; set; }
    public string? Color { get; set; }
    public string? Address { get; set; }
    public bool IsCountry { get; set; }
    public List<BlocDto>? Blocs { get; set; }
}
