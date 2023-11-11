using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PowerPlantMapAPI.Data;
using PowerPlantMapAPI.Services;

namespace PowerPlantMapAPI.Controllers;

[EnableCors]
[Route("API/[controller]/[action]")]
[ApiController]
public class PowerDataController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly IPowerDataService _powerDataService;
    
    public PowerDataController(IEmailService emailService, IPowerDataService powerDataService)
    {
        _emailService = emailService;
        _powerDataService = powerDataService;
    }

    [HttpGet("")]
    public async Task<ActionResult<IEnumerable<PowerPlantBasicsModel>>> GetPowerPlantBasics()
    {
        return await _powerDataService.GetPowerPlantBasics();
    }

    [HttpGet("")]
    public async Task<ActionResult<PowerPlantDetailsModel>> GetDetailsOfPowerPlant(string id, DateTime? date = null, DateTime? start = null, DateTime? end = null)
    {
        return await _powerDataService.GetDetailsOfPowerPlant(id, date, start, end);
    }

    [HttpGet("")]
    public async Task<IEnumerable<PowerOfPowerPlantModel>> GetPowerOfPowerPlant(string id, DateTime? date = null, DateTime? start = null, DateTime? end = null)
    {
        return await _powerDataService.GetPowerOfPowerPlant(id, date, start, end);
    }

    [HttpGet("")]
    public async Task<PowerOfPowerPlantsModel> GetPowerOfPowerPlants(DateTime? date = null, DateTime? start = null, DateTime? end = null)
    {
        return await _powerDataService.GetPowerOfPowerPlants(date, start, end);
    }

    [HttpGet("")]
    public async Task<string> InitData(DateTime? start = null, DateTime? end = null)
    {
        return await _powerDataService.InitData(start, end);
    }

    [HttpGet("")]
    public string SendTestEmail(string? to, string? subject, string? body)
    {
        return _emailService.SendEmail(to, subject, body);
    }
}