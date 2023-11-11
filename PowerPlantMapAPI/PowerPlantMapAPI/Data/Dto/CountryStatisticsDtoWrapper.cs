namespace PowerPlantMapAPI.Data.Dto;

public class CountryStatisticsDtoWrapper
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public List<CountryStatisticsDto>? Data { get; set; }
}