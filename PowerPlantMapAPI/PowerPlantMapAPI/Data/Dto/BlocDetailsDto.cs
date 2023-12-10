namespace PowerPlantMapAPI.Data.Dto;

public class BlocDetailsDto
{
    public string? BlocId { get; set; }
    public string? BlocType { get; set; }
    public int MaxBlocCapacity { get; set; }
    public int CommissionDate { get; set; }
    public int CurrentPower { get; set; }
    public int MaxPower { get; set; }
    public List<GeneratorDetailsDto>? Generators { get; set; }
}
