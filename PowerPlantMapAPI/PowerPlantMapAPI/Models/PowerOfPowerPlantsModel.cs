using PowerPlantMapAPI.Models.DTO;

namespace PowerPlantMapAPI.Models;

public class PowerOfPowerPlantsModel
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public List<PowerOfPowerPlantDto>? Data { get; set; }
}
