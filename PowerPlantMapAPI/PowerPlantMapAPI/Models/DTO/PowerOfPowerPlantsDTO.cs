namespace PowerPlantMapAPI.Models.DTO
{
    public class PowerOfPowerPlantsDTO
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<PowerOfPowerPlantDTO> Data { get; set; }
    }
}
