namespace PowerPlantMapAPI.Data.Dto;

public class BlocDataDto
{
    public string? BlocId { get; set; }
    public string? BlocType { get; set; }
    public int MaxBlocCapacity { get; set; }
    public int CommissionDate { get; set; }
    public List<GeneratorDataDto>? Generators { get; set; }
}