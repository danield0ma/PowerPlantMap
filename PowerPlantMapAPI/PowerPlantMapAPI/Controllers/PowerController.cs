using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IDateService _dateService;
        private readonly IEmailService _emailService;
        public PowerController(IPowerService powerService, IDateService dateService, IEmailService emailService)
        {
            _powerService = powerService;
            _dateService = dateService;
            _emailService = emailService;
        }

        [HttpGet("[action]")]
        public ActionResult<String> Test()
        {
            return Ok("Test");
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<FeatureDTO>>> GetPowerPlantBasics()
        {
            return await _powerService.GetPowerPlantBasics();
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<PowerPlantDetailsDTO>> GetDetailsOfPowerPlant(string Id, DateTime? Date = null, DateTime? Start = null, DateTime? End = null)
        {
            return await _powerService.GetDetailsOfPowerPlant(Id, Date, Start, End);
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<PowerStampDTO>> GetPowerOfPowerPlant(string Id, DateTime? Date = null, DateTime? Start = null, DateTime? End = null)
        {
            return await _powerService.GetPowerOfPowerPlant(Id, Date, Start, End);
        }

        [HttpGet("[action]")]
        public async Task<PowerOfPowerPlantsDTO> GetPowerOfPowerPlants(DateTime? Date = null, DateTime? Start = null, DateTime? End = null)
        {
            return await _powerService.GetPowerOfPowerPlants(Date, Start, End);
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
    }
}
