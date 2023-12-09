using PowerPlantMapAPI.Data;
using PowerPlantMapAPI.Data.Dto;

namespace PowerPlantMapAPI.Services;

public interface IEmailService
{
    Task<string?> GenerateAndSendDailyStatisticsInEmail(
        IEnumerable<CompactPowerPlantStatistics> compactPowerPlantStatistics, CountryStatisticsDtoWrapper countryStatistics,
        DateTime? day = null, DateTime? start = null, DateTime? end = null);
    List<EmailSubscriptionModel>? Get();
    EmailSubscriptionModel? GetById(Guid id);
    EmailSubscriptionModel? GetByEmail(string email);
    bool Add(string email);
    void Update(string oldEmail, string newEmail);
    void Delete(Guid id);
    public string SendEmail(string? to, string? subject, string? body);
}