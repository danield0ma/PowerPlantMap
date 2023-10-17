namespace PowerPlantMapAPI.Services;

public interface IStatisticsService
{
    Task CreateAndSendDailyStatistics();
}