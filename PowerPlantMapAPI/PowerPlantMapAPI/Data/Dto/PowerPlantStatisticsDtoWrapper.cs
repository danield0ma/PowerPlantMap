namespace PowerPlantMapAPI.Data.Dto;

public class PowerPlantStatisticsDtoWrapper
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public List<PowerPlantStatisticsDto>? Data { get; set; }
}