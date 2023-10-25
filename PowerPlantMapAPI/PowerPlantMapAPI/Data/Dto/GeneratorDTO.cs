namespace PowerPlantMapAPI.Data.Dto;

public class GeneratorDto
{
    public string? GeneratorId { get; set; }
    public int MaxCapacity { get; set; }
    public List<GeneratorPowerDto>? PastPower { get; set; }
}
