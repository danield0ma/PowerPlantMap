using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PowerPlantMapAPI.Models;
using PowerPlantMapAPI.Services;

namespace PowerPlantMapAPI.Controllers
{
    [EnableCors]
    [Route("API/[controller]/[action]")]
    [ApiController]
    public class PowerController : ControllerBase
    {
        private readonly IPowerService _powerService;
        public PowerController(IPowerService powerService)
        {
            _powerService = powerService;
        }

        [HttpGet]
        public ActionResult<string> Test()
        {
            return Ok("Test");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PowerPlantBasicsModel>>> GetPowerPlantBasics()
        {
            return await _powerService.GetPowerPlantBasics();
        }

        [HttpGet]
        public async Task<ActionResult<PowerPlantDetailsModel>> GetDetailsOfPowerPlant(string id, DateTime? date = null, DateTime? start = null, DateTime? end = null)
        {
            return await _powerService.GetDetailsOfPowerPlant(id, date, start, end);
        }

        [HttpGet]
        public async Task<IEnumerable<PowerOfPowerPlantModel>> GetPowerOfPowerPlant(string id, DateTime? date = null, DateTime? start = null, DateTime? end = null)
        {
            return await _powerService.GetPowerOfPowerPlant(id, date, start, end);
        }

        [HttpGet]
        public async Task<PowerOfPowerPlantsModel> GetPowerOfPowerPlants(DateTime? date = null, DateTime? start = null, DateTime? end = null)
        {
            return await _powerService.GetPowerOfPowerPlants(date, start, end);
        }

        [HttpGet]
        public async Task<string> InitData(DateTime? start = null, DateTime? end = null)
        {
            return await _powerService.InitData(start, end);
        }
    }
}
