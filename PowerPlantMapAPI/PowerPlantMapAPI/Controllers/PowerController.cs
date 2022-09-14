using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PowerPlantMapAPI.Models;
using System.Collections.Generic;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using PowerPlantMapAPI.Models.DTO;
using Microsoft.AspNetCore.Cors;
//using Microsoft.AspNetCore.Mvc.HttpGet;

namespace PowerPlantMapAPI.Controllers
{
    [EnableCors]
    [Route("API/[controller]")]
    [ApiController]
    public class PowerController : ControllerBase
    {
        private readonly SqlConnection _connection;
        public PowerController(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            _connection.Open();
        }

        //~PowerController()
        //{
        //    _connection.Close();
        //}

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
                properties.id = PowerPlant.id;
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

            //PowerPlantModel Paks = new PowerPlantModel
            //{
            //    id = "PKS",
            //    name = "Paks",
            //    description = "Paksi atomerőmű",
            //};

            //PowerPlantModel Paks2 = new PowerPlantModel
            //{
            //    id = "PKS2",
            //    name = "Paks 2",
            //    description = "Paksi atomerőmű 2",
            //};

            //List<PowerPlantModel> l = new List<PowerPlantModel>();
            //l.Add(Paks);
            //l.Add(Paks2);

            //return l;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<string>> getData()
        {
            var httpClient = new HttpClient();
            
            string apiResponse = "";
            var response = await httpClient.GetAsync("https://transparency.entsoe.eu/api?securityToken=a5fb8873-ad26-4972-a5f4-62e2e069f782&documentType=A73&processType=A16&psrType=B14&in_Domain=10YFR-RTE------C&periodStart=202208252000&periodEnd=202208252100");
            
            apiResponse = await response.Content.ReadAsStringAsync();
            //reservationList = JsonConvert.DeserializeObject<List<Reservation>>(apiResponse);
            
            return apiResponse;
        }


        [HttpGet("[action]")]
        public async Task<ActionResult<PowerPlantModel>> getDetailsOfPowerPlant(string id)
        {
            ReactorModel model1 = new ReactorModel()
            {
                ReactorId = "1",
                ReactorType = "VVER-440",
                ReactorName = "Paks 1",
                MaxPower = 500
            };

            ReactorModel model2 = new ReactorModel()
            {
                ReactorId = "2",
                ReactorType = "VVER-440",
                ReactorName = "Paks 2",
                MaxPower = 500
            };

            List < ReactorModel > l = new List<ReactorModel>();
            l.Add(model1);
            l.Add(model2);

            PowerPlantModel Paks = new PowerPlantModel
            {
                id = "PKS",
                name = "Paks",
                description = "Paksi atomerőmű",
                reactors = l
            };

            return Paks;
        }
    }
}
