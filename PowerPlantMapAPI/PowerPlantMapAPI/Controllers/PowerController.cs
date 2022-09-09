using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PowerPlantMapAPI.Models;
using System.Collections.Generic;
//using Microsoft.AspNetCore.Mvc.HttpGet;

namespace PowerPlantMapAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PowerController : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<ActionResult<List<PowerPlantModel>>> getPowerPlantBasics()
        {
            PowerPlantModel Paks = new PowerPlantModel
            {
                id = "PKS",
                name = "Paks",
                description = "Paksi atomerőmű",
            };

            PowerPlantModel Paks2 = new PowerPlantModel
            {
                id = "PKS2",
                name = "Paks 2",
                description = "Paksi atomerőmű 2",
            };

            List<PowerPlantModel> l = new List<PowerPlantModel>();
            l.Add(Paks);
            l.Add(Paks2);

            return l;
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
