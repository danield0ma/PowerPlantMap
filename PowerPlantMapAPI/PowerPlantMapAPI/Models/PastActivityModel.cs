namespace PowerPlantMapAPI.Models
{
    public class PastActivityModel
    {
        public string GeneratorID { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int ActualPower { get; set; }
    }
}
