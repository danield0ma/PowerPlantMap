using PowerPlantMapAPI.Data.Dto;

namespace PowerPlantMapAPI.Services;

public interface IEmailService
{
    Task<string?> GenerateAndSendDailyStatisticsInEmail(
        PowerPlantStatisticsDtoWrapper powerPlantStatistics, CountryStatisticsDtoWrapper countryStatistics,
        DateTime? day = null, DateTime? start = null, DateTime? end = null);
    public string SendEmail(string? to, string? subject, string? body);
}