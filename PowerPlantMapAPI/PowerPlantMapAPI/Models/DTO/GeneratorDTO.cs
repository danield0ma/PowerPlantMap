namespace PowerPlantMapAPI.Models.DTO
{
    public class GeneratorDTO
    {
        public string GeneratorID { get; set; }
        public int MaxCapacity { get; set; }
        public List<int> CurrentPower { get; set; }
    }
}
