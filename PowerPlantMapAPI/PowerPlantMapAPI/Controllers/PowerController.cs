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
        public async Task<ActionResult<XmlNode>> getData()
        {
            string query = "https://transparency.entsoe.eu/api?securityToken=a5fb8873-ad26-4972-a5f4-62e2e069f782&documentType=A73&processType=A16&psrType=B14&in_Domain=10YFR-RTE------C&periodStart=202208252000&periodEnd=202208252100";

            var httpClient = new HttpClient();
            
            string apiResponse = "";
            var response = await httpClient.GetAsync(query);

            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;


            apiResponse = await response.Content.ReadAsStringAsync();

            doc.Load(new StringReader(apiResponse));

            //foreach(XmlNode node in doc.ChildNodes[10])
            //System.Diagnostics.Debug.WriteLine("Child Nodes count: " + doc.ChildNodes[2].ChildNodes.Count);

            int sum = 0;
            
            for (int i = 20; i < doc.ChildNodes[2].ChildNodes.Count; i++)
            {
                XmlNode node = doc.ChildNodes[2].ChildNodes[i];
                
                if(node.ChildNodes.Count != 0)
                {
                    XmlNode first = node.ChildNodes[15];
                    //System.Diagnostics.Debug.WriteLine("15: " + first.InnerXml);
                    //System.Diagnostics.Debug.WriteLine("15: " + first.ChildNodes.Count);
                    //System.Diagnostics.Debug.WriteLine("15: " + first.ChildNodes[3].InnerXml);
                    //System.Diagnostics.Debug.WriteLine("15: " + first.ChildNodes[3].ChildNodes.Count);
                    System.Diagnostics.Debug.WriteLine("15: " + first.ChildNodes[3].ChildNodes[3].InnerXml);

                    XmlNode second = node.ChildNodes[17];
                    //System.Diagnostics.Debug.WriteLine("17: " + second.InnerXml);
                    //System.Diagnostics.Debug.WriteLine("17: " + second.ChildNodes.Count);
                    //System.Diagnostics.Debug.WriteLine("17: " + second.ChildNodes[5].InnerXml);
                    //System.Diagnostics.Debug.WriteLine("17: " + second.ChildNodes[5].ChildNodes.Count);
                    //System.Diagnostics.Debug.WriteLine("17: " + second.ChildNodes[5].ChildNodes[3].InnerXml);
                    int p = Int32.Parse(second.ChildNodes[5].ChildNodes[3].InnerXml);
                    System.Diagnostics.Debug.WriteLine("17: " + p);
                    sum += p;
                }

                //System.Diagnostics.Debug.WriteLine("Inner node count: " + node.ChildNodes.Count);
                //System.Diagnostics.Debug.WriteLine("I:" + i);
            }

            System.Diagnostics.Debug.WriteLine("Sum: " + sum);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("asd", query);
            string xPathString = "//asd:GL_MarketDocument/asd:TimeSeries/asd:MktPSRType/asd:PowerSystemResources[@name='BELLEVILLE 1']";
            XmlNode xmlNode = doc.DocumentElement.SelectSingleNode(xPathString, nsmgr);
            return xmlNode;

            //reservationList = JsonConvert.DeserializeObject<List<Reservation>>(apiResponse);

            //return apiResponse;
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
