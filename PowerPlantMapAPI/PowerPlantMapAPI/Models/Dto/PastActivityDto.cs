namespace PowerPlantMapAPI.Models.DTO
{
    public class PastActivityDto
    {
        public string? GeneratorId { get; set; }
        public DateTime PeriodStart { get; set; }
        public int ActualPower { get; set; }
    }
}
