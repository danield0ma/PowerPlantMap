using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PowerPlantMapAPI.Data;
using PowerPlantMapAPI.Data.Dto;
using PowerPlantMapAPI.Services;

namespace PowerPlantMapAPI.Controllers;

/// <summary>
/// Generate and send statistics emails
/// </summary>
[EnableCors]
[Route("api/[controller]/[action]")]
[ApiController]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;
    private readonly IEmailService _emailService;

    /// <summary>
    /// Instantiate the dependencies
    /// </summary>
    public StatisticsController(IStatisticsService statisticsService, IEmailService emailService)
    {
        _statisticsService = statisticsService;
        _emailService = emailService;
    }

    /// <summary>
    /// Generate detailed statistics of every power plant including their generators
    /// </summary>
    /// <param name="day">Optional, one day for the interval</param>
    /// <param name="start">Optional, start of a custom time interval</param>
    /// <param name="end">Optional, end of a custom time interval</param>
    /// <returns>Object of detailed power plant statistics</returns>
    [HttpGet("")]
    public async Task<PowerPlantStatisticsDtoWrapper> GeneratePowerPlantStatistics(DateTime? day, DateTime? start,
        DateTime? end)
    {
        return await _statisticsService.GenerateDailyPowerPlantStatistics(day, start, end);
    }
    
    /// <summary>
    /// Generate compact statistics of every power plant
    /// </summary>
    /// <param name="day">Optional, one day for the interval</param>
    /// <param name="start">Optional, start of a custom time interval</param>
    /// <param name="end">Optional, end of a custom time interval</param>
    /// <returns>Object of compact power plant statistics</returns>
    [HttpGet("")]
    public async Task<IEnumerable<CompactPowerPlantStatistics>> GenerateCompactPowerPlantStatistics(DateTime? day, DateTime? start,
        DateTime? end)
    {
        return await _statisticsService.GenerateCompactPowerPlantStatistics(day, start, end);
    }

    /// <summary>
    /// Generate statistics of every country trades in the given time period
    /// </summary>
    /// <param name="day">Optional, one day for the interval</param>
    /// <param name="start">Optional, start of a custom time interval</param>
    /// <param name="end">Optional, end of a custom time interval</param>
    /// <returns>Object of country trade statistics</returns>
    [HttpGet("")]
    public async Task<CountryStatisticsDtoWrapper> GenerateCountryStatistics(DateTime? day, DateTime? start, DateTime? end)
    {
        return await _statisticsService.GenerateDailyCountryStatistics(day, start, end);
    }

    /// <summary>
    /// Send statistics email to every subscriber
    /// </summary>
    /// <param name="day">Optional, one day for the interval</param>
    /// <param name="start">Optional, start of a custom time interval</param>
    /// <param name="end">Optional, end of a custom time interval</param>
    /// <returns>Result of the email sendings</returns>
    [HttpGet("")]
    public async Task<string?> GenerateAndSendStatistics(DateTime? day, DateTime? start, DateTime? end)
    {
        var powerPlantStatistics = await _statisticsService.GenerateCompactPowerPlantStatistics(day, start, end);
        var countryStatistics = await _statisticsService.GenerateDailyCountryStatistics(day, start, end);
        return await _emailService.GenerateAndSendDailyStatisticsInEmail(powerPlantStatistics, countryStatistics, day,
            start, end);
    }
}