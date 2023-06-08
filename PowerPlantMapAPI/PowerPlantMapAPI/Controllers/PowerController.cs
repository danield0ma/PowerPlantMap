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
        public PowerController(IPowerService powerService, IDateService dateService)
        {
            _powerService = powerService;
            _dateService = dateService;
        }

        [HttpGet("[action]")]
        public ActionResult<String> Test()
        {
            return Ok("Test");
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<FeatureDTO>>> getPowerPlantBasics()
        {
            return await _powerService.GetPowerPlantBasics();
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<PowerPlantDetailsDTO>> getDetailsOfPowerPlant(string id, DateTime? date = null)
        {
            return await _powerService.GetDetailsOfPowerPlant(id, date);
        }

        [HttpGet("[action]")]
        public async Task<PowerOfPowerPlantsDTO> GetPowerOfPowerPlants(DateTime? date = null)
        {
            return await _powerService.GetPowerOfPowerPlants(date);
        }

        [HttpGet("[action]")]
        public async Task<string> InitData(DateTime? start = null, DateTime? end = null)
        {
            return await _powerService.InitData(start, end);
        }

        //[HttpGet("[action]")]
        //public async Task<CurrentLoadDTO> GetCurrentLoad()
        //{
        //    List<DateTime> startend = await _dateService.GetLastDataTime();
        //    CurrentLoadDTO apiResponse = await _powerService.GetCurrentLoad(_dateService.EditTime(startend[0]), _dateService.EditTime(startend[1]));
        //    return apiResponse;
        //}

        //[HttpGet("[action]")]
        //public async Task<IEnumerable<CurrentLoadDTO>> GetLoadHistory()
        //{
        //    List<DateTime> startend = await _dateService.GetLastDataTime();
        //    return await _powerService.GetLoadHistory(startend[0], startend[1]);
        //}
    }
}
