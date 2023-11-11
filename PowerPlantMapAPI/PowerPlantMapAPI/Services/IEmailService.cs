using PowerPlantMapAPI.Models.DTO;

namespace PowerPlantMapAPI.Services
{
    public interface IEmailService
    {
        Task<string?> GenerateAndSendDailyStatisticsInEmail(List<PowerPlantStatisticsDto> powerPlantStatistics,
            List<CountryStatisticsDto> countryStatistics);
        public string SendEmail(string? to, string? subject, string? body);
    }
}