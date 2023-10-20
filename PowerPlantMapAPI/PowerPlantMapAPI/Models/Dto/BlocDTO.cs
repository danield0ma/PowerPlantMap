namespace PowerPlantMapAPI.Models.DTO;

public class BlocDto
{
    public string? BlocId { get; set; }
    public string? BlocType { get; set; }
    public int MaxBlocCapacity { get; set; }
    public int CommissionDate { get; set; }
    public int CurrentPower { get; set; }
    public int MaxPower { get; set; }
    public List<GeneratorDto>? Generators { get; set; }
}
