﻿using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PowerPlantMapAPI.Models;
using PowerPlantMapAPI.Models.DTO;
using PowerPlantMapAPI.Services;

namespace PowerPlantMapAPI.Controllers
{
    [EnableCors]
    [Route("API/[controller]")]
    [ApiController]
    public class PowerController : ControllerBase
    {
        private readonly IPowerService _powerService;
        private readonly IEmailService _emailService;
        private readonly IStatisticsService _statisticsService;
        public PowerController(IPowerService powerService, IEmailService emailService, IStatisticsService statisticsService)
        {
            _powerService = powerService;
            _emailService = emailService;
            _statisticsService = statisticsService;
        }

        [HttpGet("[action]")]
        public ActionResult<String> Test()
        {
            return Ok("Test");
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<PowerPlantBasicsModel>>> GetPowerPlantBasics()
        {
            return await _powerService.GetPowerPlantBasics();
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<PowerPlantDetailsModel>> GetDetailsOfPowerPlant(string id, DateTime? date = null, DateTime? start = null, DateTime? end = null)
        {
            return await _powerService.GetDetailsOfPowerPlant(id, date, start, end);
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<PowerOfPowerPlantModel>> GetPowerOfPowerPlant(string id, DateTime? date = null, DateTime? start = null, DateTime? end = null)
        {
            return await _powerService.GetPowerOfPowerPlant(id, date, start, end);
        }

        [HttpGet("[action]")]
        public async Task<PowerOfPowerPlantsModel> GetPowerOfPowerPlants(DateTime? date = null, DateTime? start = null, DateTime? end = null)
        {
            return await _powerService.GetPowerOfPowerPlants(date, start, end);
        }

        [HttpGet("[action]")]
        public async Task<string> InitData(DateTime? start = null, DateTime? end = null)
        {
            return await _powerService.InitData(start, end);
        }

        [HttpGet("[action]")]
        public string SendTestEmail(string? to, string? subject, string? body)
        {
            return _emailService.SendEmail(to, subject, body);
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
