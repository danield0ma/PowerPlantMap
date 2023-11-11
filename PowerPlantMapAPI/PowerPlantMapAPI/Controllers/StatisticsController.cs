using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PowerPlantMapAPI.Data.Dto;
using PowerPlantMapAPI.Helpers;
using PowerPlantMapAPI.Services;

namespace PowerPlantMapAPI.Controllers;

[EnableCors]
[Route("API/[controller]")]
[ApiController]
public class StatisticsController: ControllerBase
{
    private readonly IStatisticsService _statisticsService;
    private readonly IEmailService _emailService;
    private readonly IDateHelper _dateHelper;

    public StatisticsController(IStatisticsService statisticsService, IEmailService emailService, IDateHelper dateHelper)
    {
        _statisticsService = statisticsService;
        _emailService = emailService;
        _dateHelper = dateHelper;
    }

    [HttpGet("[action]")]
    public async Task<PowerPlantStatisticsDtoWrapper> GetDailyPowerPlantStatistics(DateTime? day, DateTime? start, DateTime? end)
    {
        return await _statisticsService.GenerateDailyPowerPlantStatistics(day, start, end);
    }

    [HttpGet("[action]")]
    public async Task<CountryStatisticsDtoWrapper> GetDailyCountryStatistics(DateTime? day, DateTime? start, DateTime? end)
    {
        return await _statisticsService.GenerateDailyCountryStatistics(day, start, end);
    }

    [HttpGet("[action]")]
    public async Task<string?> GenerateAndSendStatistics(DateTime? day, DateTime? start, DateTime? end)
    {
        var powerPlantStatistics = await _statisticsService.GenerateDailyPowerPlantStatistics(day, start, end);
        var countryStatistics = await _statisticsService.GenerateDailyCountryStatistics(day, start, end);
        return await _emailService.GenerateAndSendDailyStatisticsInEmail(powerPlantStatistics, countryStatistics, day, start, end);
    }
}