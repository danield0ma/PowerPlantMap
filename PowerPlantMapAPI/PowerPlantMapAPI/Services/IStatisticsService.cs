using PowerPlantMapAPI.Models.DTO;

namespace PowerPlantMapAPI.Services;

public interface IStatisticsService
{
    Task<List<DailyStatisticsDto>> CreateAndSendDailyStatistics();
}