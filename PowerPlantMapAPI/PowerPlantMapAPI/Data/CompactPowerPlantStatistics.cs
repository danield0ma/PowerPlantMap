namespace PowerPlantMapAPI.Data;

public class CompactPowerPlantStatistics
{
    public string? PowerPlantId { get; set; }
    public string? PowerPlantName { get; set; }
    public string? PowerPlantDescription { get; set; }
    public string? Image { get; set; }
    public int MaxPower { get; set; }
    public double AveragePower { get; set; }
    public double GeneratedEnergy { get; set; }
    public double AverageUsage { get; set; }
}