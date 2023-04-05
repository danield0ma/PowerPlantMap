using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PowerPlantMapAPI.Models;
using System.Collections.Generic;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using PowerPlantMapAPI.Models.DTO;
using Microsoft.AspNetCore.Cors;
using System.Xml;
using System.Collections;
using PowerPlantMapAPI.Services;
using System.Reflection.Emit;

namespace PowerPlantMapAPI.Controllers
{
    [EnableCors]
    [Route("API/[controller]")]
    [ApiController]
    public class PowerController : ControllerBase
    {
        private readonly SqlConnection _connection;
        private readonly IPowerService _service;
        public PowerController(IConfiguration configuration, IPowerService powerService)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            _connection.Open();
            _service = powerService;
        }

        [HttpGet("[action]")]
        public async Task<CurrentLoadDTO> GetCurrentLoad()
        {
            List<DateTime> startend = await _service.GetStartAndEnd(false);
            CurrentLoadDTO apiResponse = await _service.GetCurrentLoad(_service.EditTime(startend[0]), _service.EditTime(startend[1]));
            return apiResponse;
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<CurrentLoadDTO>> GetLoadHistory()
        {
            List<DateTime> startend = await _service.GetStartAndEnd(false);
            return await _service.GetLoadHistory(startend[0], startend[1]);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<FeatureModel>>> getPowerPlantBasics()
        {
            return await _service.getPowerPlantBasics();
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<PowerPlantDetailsModel>> getDetailsOfPowerPlant(string id, DateTime? date = null)
        {
            return await _service.getDetailsOfPowerPlant(id, date);
        }

        [HttpGet("[action]")]
        public async Task<PowerOfPowerPlantsModel> GetPowerOfPowerPlants(DateTime? date = null)
        {
            return await _service.GetPowerOfPowerPlants();
        }

        [HttpGet("[action]")]
        public async Task<string> InitData(DateTime? start = null, DateTime? end = null)
        {
            return await _service.InitData(start, end);
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<PowerDTO>> GetPsrTypeData(string documentType)
        { //A73 - generation unit, A75 - generation type
            List<DateTime> TimeStamps = await _service.GetStartAndEnd(true);
            string StartTime = _service.EditTime(TimeStamps[0]);
            string EndTime = _service.EditTime(TimeStamps[1]);
            return await _service.getPPData(documentType, StartTime, EndTime);
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<PowerDTO>> GetImportData(bool export = false)
        {
            List<DateTime> TimeStamps = await _service.GetStartAndEnd(true);
            string StartTime = _service.EditTime(TimeStamps[0]);
            string EndTime = _service.EditTime(TimeStamps[1]);
            return await _service.getImportData(export, StartTime, EndTime);
        }

        [HttpGet("[action]")]
        public async Task<bool> Test(DateTime Start, DateTime End)
        {
            var parameters = new { GID = "PA_gép1", start = Start, end = End };
            List<PastActivityModel> PastActivity = (List<PastActivityModel>)await _connection.QueryAsync<PastActivityModel>
            ("GetPastActivity", parameters, commandType: CommandType.StoredProcedure);

            System.Diagnostics.Debug.WriteLine(PastActivity[0]);
            System.Diagnostics.Debug.WriteLine("HOSSZA: " + PastActivity.Count);

            return true;
        }
    }
}
