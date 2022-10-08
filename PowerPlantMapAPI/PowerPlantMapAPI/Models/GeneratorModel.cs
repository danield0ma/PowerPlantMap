namespace PowerPlantMapAPI.Models
{
    public class GeneratorModel
    {
        public string GeneratorID { get; set; }
        public int MaxCapacity { get; set; }
        public List<int> CurrentPower { get; set; }
    }
}
