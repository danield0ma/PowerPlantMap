using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PowerPlantMapAPI.Models.DTO;
using PowerPlantMapAPI.Services;

namespace PowerPlantMapAPI.Controllers;

public class StatisticsController: ControllerBase
{
    [EnableCors]
    [Route("API/[controller]")]
    [ApiController]
    public class PowerController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IEmailService _emailService;

        public PowerController(IStatisticsService statisticsService, IEmailService emailService)
        {
            _statisticsService = statisticsService;
            _emailService = emailService;
        }

        [HttpGet("[action]")]
        public async Task<List<PowerPlantStatisticsDto>> GetDailyPowerPlantStatistics()
        {
            return await _statisticsService.GenerateDailyPowerPlantStatistics();
        }

        [HttpGet("[action]")]
        public async Task<List<CountryStatisticsDto>> GetDailyCountryStatistics()
        {
            return await _statisticsService.GenerateDailyCountryStatistics();
        }

        [HttpGet("[action]")]
        public async Task<string?> GenerateAndSendDailyStatistics()
        {
            var powerPlantStatistics = await _statisticsService.GenerateDailyPowerPlantStatistics();
            var countryStatistics = await _statisticsService.GenerateDailyCountryStatistics();
            return await _emailService.GenerateAndSendDailyStatisticsInEmail(powerPlantStatistics, countryStatistics);
        }
    }
}