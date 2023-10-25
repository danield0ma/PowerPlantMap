using PowerPlantMapAPI.Data.Dto;

namespace PowerPlantMapAPI.Data;

public class PowerOfPowerPlantsModel
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public List<PowerOfPowerPlantDto>? Data { get; set; }
}
