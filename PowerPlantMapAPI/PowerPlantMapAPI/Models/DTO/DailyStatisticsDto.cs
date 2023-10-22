namespace PowerPlantMapAPI.Models.DTO;

public class DailyStatisticsDto
{
    public string? GeneratorName { get; set; }
    public int MaxPower { get; set; }
    
    public double AveragePower { get; set; }
    public double GeneratedEnergy { get; set; }
    public double AverageUsage { get; set; }
}