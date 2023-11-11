using PowerPlantMapAPI.Models.DTO;

namespace PowerPlantMapAPI.Services;

public interface IStatisticsService
{
    Task<List<PowerPlantStatisticsDto>> GenerateDailyPowerPlantStatistics();

    Task<List<CountryStatisticsDto>> GenerateDailyCountryStatistics();
}