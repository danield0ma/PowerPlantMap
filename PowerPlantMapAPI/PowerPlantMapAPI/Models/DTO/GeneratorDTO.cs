namespace PowerPlantMapAPI.Models.DTO
{
    public class GeneratorDTO
    {
        public string GeneratorID { get; set; }
        public int MaxCapacity { get; set; }
        public List<GeneratorPowerDTO> PastPower { get; set; }
    }
}
