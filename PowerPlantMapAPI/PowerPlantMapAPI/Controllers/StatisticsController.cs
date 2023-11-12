using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PowerPlantMapAPI.Data;
using PowerPlantMapAPI.Data.Dto;
using PowerPlantMapAPI.Services;

namespace PowerPlantMapAPI.Controllers;

[EnableCors]
[Route("API/[controller]/[action]")]
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
    public async Task<PowerPlantStatisticsDtoWrapper> GetPowerPlantStatistics(DateTime? day, DateTime? start,
        DateTime? end)
    {
        return await _statisticsService.GenerateDailyPowerPlantStatistics(day, start, end);
    }

    [HttpGet("")]
    public async Task<CountryStatisticsDtoWrapper> GetCountryStatistics(DateTime? day, DateTime? start, DateTime? end)
    {
        return await _statisticsService.GenerateDailyCountryStatistics(day, start, end);
    }

    [HttpGet("")]
    public async Task<string?> GenerateAndSendStatistics(DateTime? day, DateTime? start, DateTime? end)
    {
        var powerPlantStatistics = await _statisticsService.GenerateDailyPowerPlantStatistics(day, start, end);
        var countryStatistics = await _statisticsService.GenerateDailyCountryStatistics(day, start, end);
        return await _emailService.GenerateAndSendDailyStatisticsInEmail(powerPlantStatistics, countryStatistics, day,
            start, end);
    }

    [Authorize]
    [HttpGet("")]
    public List<EmailSubscriptionModel>? GetEmailSubscriptions()
    {
        return _emailService.Get();
    }

    [Authorize]
    [HttpGet("")]
    public EmailSubscriptionModel? GetEmailSubscriptionById(Guid id)
    {
        return _emailService.GetById(id);
    }

    [Authorize]
    [HttpGet("")]
    public EmailSubscriptionModel? GetEmailSubscriptionByEmail(string email)
    {
        return _emailService.GetByEmail(email);
    }

    [HttpPost("")]
    public ActionResult AddNewEmailSubscription(string email)
    {
        try
        {
            _emailService.Add(email);
            return Ok();
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e);
            return BadRequest();
        }
    }

    [HttpPut("")]
    public ActionResult UpdateEmailSubscription(string oldEmail, string newEmail)
    {
        try
        {
            _emailService.Update(oldEmail, newEmail);
            return Ok();
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e);
            return BadRequest();
        }
    }

    [HttpDelete("")]
    public ActionResult DeleteEmailSubscription(string email)
    {
        try
        {
            _emailService.Delete(email);
            return Ok();
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e);
            return BadRequest();
        }
    }
}