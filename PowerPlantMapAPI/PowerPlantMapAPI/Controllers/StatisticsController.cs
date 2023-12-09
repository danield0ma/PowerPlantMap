using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PowerPlantMapAPI.Data;
using PowerPlantMapAPI.Data.Dto;
using PowerPlantMapAPI.Services;

namespace PowerPlantMapAPI.Controllers;

[EnableCors]
[Route("api/[controller]/[action]")]
[ApiController]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;
    private readonly IEmailService _emailService;

    public StatisticsController(IStatisticsService statisticsService, IEmailService emailService)
    {
        _statisticsService = statisticsService;
        _emailService = emailService;
    }

    [HttpGet("")]
    public async Task<PowerPlantStatisticsDtoWrapper> GeneratePowerPlantStatistics(DateTime? day, DateTime? start,
        DateTime? end)
    {
        return await _statisticsService.GenerateDailyPowerPlantStatistics(day, start, end);
    }
    
    [HttpGet("")]
    public async Task<IEnumerable<CompactPowerPlantStatistics>> GenerateCompactPowerPlantStatistics(DateTime? day, DateTime? start,
        DateTime? end)
    {
        return await _statisticsService.GenerateCompactPowerPlantStatistics(day, start, end);
    }

    [HttpGet("")]
    public async Task<CountryStatisticsDtoWrapper> GenerateCountryStatistics(DateTime? day, DateTime? start, DateTime? end)
    {
        return await _statisticsService.GenerateDailyCountryStatistics(day, start, end);
    }

    [HttpGet("")]
    public async Task<string?> GenerateAndSendStatistics(DateTime? day, DateTime? start, DateTime? end)
    {
        var powerPlantStatistics = await _statisticsService.GenerateCompactPowerPlantStatistics(day, start, end);
        var countryStatistics = await _statisticsService.GenerateDailyCountryStatistics(day, start, end);
        return await _emailService.GenerateAndSendDailyStatisticsInEmail(powerPlantStatistics, countryStatistics, day,
            start, end);
    }
    
    [HttpGet("")]
    public string SendTestEmail(string? to, string? subject, string? body)
    {
        return _emailService.SendEmail(to, subject, body);
    }
}