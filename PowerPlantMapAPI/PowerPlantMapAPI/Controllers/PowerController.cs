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
            List<PowerPlantDataDTO> PowerPlants = (List<PowerPlantDataDTO>)await _connection.QueryAsync<PowerPlantDataDTO>("[PowerPlantBasics]", CommandType.StoredProcedure);
            List<FeatureModel> PowerPlantBasics = new List<FeatureModel>();

            foreach(var PowerPlant in PowerPlants)
            {
                FeatureModel feature = new FeatureModel();
                feature.Type = "Feature";

                FeaturePropertyModelDTO properties = new FeaturePropertyModelDTO();
                properties.id = PowerPlant.PowerPlantID;
                properties.name = PowerPlant.name;
                properties.description = PowerPlant.description;
                properties.img = PowerPlant.image;
                feature.properties = properties;

                FeatureGeometryModelDTO geometry = new FeatureGeometryModelDTO();
                geometry.type = "Point";
                List<float> coordinates = new List<float>();
                coordinates.Add(PowerPlant.latitude);
                coordinates.Add(PowerPlant.longitude);                
                geometry.coordinates = coordinates;
                feature.geometry = geometry;

                PowerPlantBasics.Add(feature);
            }

            return PowerPlantBasics;
        }

        private async Task<BasicsOfPowerPlantDTO> GetBasicsOfPowerPlant(string id)
        {
            var parameters = new { id = id };
            List<PowerPlantDataDTO> PP = (List<PowerPlantDataDTO>)await 
                _connection.QueryAsync<PowerPlantDataDTO>
                ("[GetBasicsOfPowerPlant]", parameters, commandType: CommandType.StoredProcedure);

            PowerPlantDataDTO PowerPlant = PP[0];
            BasicsOfPowerPlantDTO b = new BasicsOfPowerPlantDTO();
            b.id = PowerPlant.PowerPlantID;
            b.name = PowerPlant.name;
            b.description = PowerPlant.description;
            b.OperatorCompany = PowerPlant.OperatorCompany;
            b.webpage = PowerPlant.webpage;
            b.Color = PowerPlant.Color;
            b.Address = PowerPlant.Address;

            return b;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<PowerPlantDetailsModel>> getDetailsOfPowerPlant(string id)
        {
            PowerPlantDetailsModel PowerPlant = new PowerPlantDetailsModel();
            PowerPlant.PowerPlantID = id;
            BasicsOfPowerPlantDTO basics = await GetBasicsOfPowerPlant(id);

            PowerPlant.name = basics.name;
            PowerPlant.description = basics.description;
            PowerPlant.OperatorCompany = basics.OperatorCompany;
            PowerPlant.webpage = basics.webpage;
            PowerPlant.Color = basics.Color;
            PowerPlant.Address = basics.Address;

            var parameters = new { PowerPlantID = id };
            List<PowerPlantDetailsDTO> PowerPlantDetails = 
                (List<PowerPlantDetailsDTO>)await _connection.
                QueryAsync<PowerPlantDetailsDTO>("GetPowerPlantDetails", 
                    parameters, commandType: CommandType.StoredProcedure);

            List<DateTime> TimeStamps = await _service.GetStartAndEnd(false);
            PowerPlant.DataStart = TimeStamps[0];
            PowerPlant.DataEnd = TimeStamps[1];

            int PPMaxPower = 0, PPCurrentPower = 0;
            List<BlocModel> Blocs = new List<BlocModel>();
            for (int i = 0; i < PowerPlantDetails.Count; i += 0)
            {
                BlocModel Bloc = new BlocModel();
                Bloc.BlocID = PowerPlantDetails[i].BlocId;
                Bloc.BlocType = PowerPlantDetails[i].BlocType;
                Bloc.MaxBlocCapacity = PowerPlantDetails[i].MaxBlocCapacity;
                Bloc.ComissionDate = PowerPlantDetails[i].ComissionDate;
                
                List<GeneratorModel> Generators = new List<GeneratorModel>();
                int CurrentPower = 0, MaxPower = 0;
                while (i < PowerPlantDetails.Count && PowerPlantDetails[i].BlocId == Bloc.BlocID)
                {
                    GeneratorModel Generator = new GeneratorModel();
                    Generator.GeneratorID = PowerPlantDetails[i].GeneratorID;
                    Generator.MaxCapacity = PowerPlantDetails[i].MaxCapacity;
                    Generator.CurrentPower = await GetGeneratorPower(Generator.GeneratorID, TimeStamps[0], TimeStamps[1]);
                    Generators.Add(Generator);
                    CurrentPower += Generator.CurrentPower[0];
                    MaxPower += Generator.MaxCapacity;
                    i++;
                }
                Bloc.CurrentPower = CurrentPower;
                Bloc.MaxPower = MaxPower;
                Bloc.Generators = Generators;
                Blocs.Add(Bloc);

                PPCurrentPower += CurrentPower;
                PPMaxPower += MaxPower;
            }
            PowerPlant.Blocs = Blocs;
            PowerPlant.CurrentPower = PPCurrentPower;
            PowerPlant.MaxPower = PPMaxPower;
            return PowerPlant;
        }

        private async Task<List<int>> GetGeneratorPower(string generator, DateTime start, DateTime end)
        //TODO GeneratorPowerDTO-val térjen vissza, hogy ne a frontenden kelljen az időket hozzáigazítani
        {
            var parameters = new { GID = generator, start = start, end = end };
            List<PastActivityModel> PastActivity = (List<PastActivityModel>)await _connection.QueryAsync<PastActivityModel>
                ("GetPastActivity", parameters, commandType: CommandType.StoredProcedure);

            List<int> power = new List<int>();
            foreach(var Activity in PastActivity)
            {
                power.Add(Activity.ActualPower);
            }

            for(int i = power.Count; i < 97; i++)
            {
                power.Add(0);
            }
            return power;
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<PowerDTO>> GetPowerOfPowerPlants()
        {
            List<string> PowerPlants = (List<string>)await _connection.QueryAsync<string>
                    ("GetPowerPlants", commandType: CommandType.StoredProcedure);

            List<DateTime> TimeStamps = await _service.GetStartAndEnd(false);

            List<PowerDTO> PowerOfPPs = new List<PowerDTO>();

            foreach (string PowerPlant in PowerPlants)
            {
                PowerDTO Power = new PowerDTO();
                Power.PowerPlantBloc = PowerPlant;
                Power.Power = new List<int>();

                for (int i = 0; i < 97; i++)
                {
                    Power.Power.Add(0);
                }

                var parameters = new { PPID = PowerPlant };
                List<string> Generators = (List<string>)await _connection.QueryAsync<string>
                    ("GetGeneratorsOfPowerPlant", parameters, commandType: CommandType.StoredProcedure);

                foreach (string Generator in Generators)
                {
                    List<int> Gen = await GetGeneratorPower(Generator, TimeStamps[0], TimeStamps[1]);
                    for (int i = 0; i < 97; i++)
                    {
                        Power.Power[i] += Gen[i];
                    }
                }

                PowerOfPPs.Add(Power);
            }

            return PowerOfPPs;
        }

        private string getTime(int diff)
        {
            string now = Convert.ToString(DateTime.Now.Year);
            if (DateTime.Now.Month < 10) { now += "0"; }
            now += Convert.ToString(DateTime.Now.Month);
            if (DateTime.Now.Day < 10) { now += "0"; }
            now += Convert.ToString(DateTime.Now.Day);
            if (DateTime.Now.Hour - diff < 10) { now += "0"; }
            now += Convert.ToString(DateTime.Now.Hour - diff) + "00";
            return now;
        }

        [HttpGet("[action]")]
        public async Task<string> InitData()
        {
            return await _service.InitData();
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
    }
}
