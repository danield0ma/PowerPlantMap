namespace PowerPlantMapAPI.Data.Dto;

public class AllDataOfPowerPlantDto
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
    public string? BlocId { get; set; }
    public string? BlocType { get; set; }
    public int MaxBlocCapacity { get; set; }
    public int CommissionDate { get; set; }
    public string? GeneratorId { get; set; }
    public int MaxCapacity { get; set; }
}