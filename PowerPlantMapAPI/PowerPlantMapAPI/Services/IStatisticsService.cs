using PowerPlantMapAPI.Data.Dto;

namespace PowerPlantMapAPI.Services;

public interface IStatisticsService
{
    Task<PowerPlantStatisticsDtoWrapper> GenerateDailyPowerPlantStatistics(DateTime? day = null, DateTime? start = null, DateTime? end = null);
    Task<CountryStatisticsDtoWrapper> GenerateDailyCountryStatistics(DateTime? day = null, DateTime? start = null, DateTime? end = null);
}