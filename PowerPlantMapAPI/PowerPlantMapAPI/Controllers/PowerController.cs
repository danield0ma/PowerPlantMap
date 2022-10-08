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

            var parameters = new { PowerPlantID = id };
            List<PowerPlantDetailsDTO> PowerPlantDetails = 
                (List<PowerPlantDetailsDTO>)await _connection.
                QueryAsync<PowerPlantDetailsDTO>("GetPowerPlantDetails", 
                    parameters, commandType: CommandType.StoredProcedure);
            
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
                    Generator.CurrentPower = await getGeneratorPower(Generator.GeneratorID);
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

        private async Task<List<int>> getGeneratorPower(string generator)
        {
            var parameters = new { GID = generator };
            List<PastActivityModel> PastActivity = (List<PastActivityModel>)await _connection.QueryAsync<PastActivityModel>
                ("GetPastActivity", parameters, commandType: CommandType.StoredProcedure);

            List<int> power = new List<int>();
            foreach(var Activity in PastActivity)
            {
                power.Add(Activity.ActualPower);
            }
            return power;

            //List<PowerDTO> data = (List<PowerDTO>)await getData();
            //foreach(PowerDTO gen in data)
            //{
            //    if(gen.PowerPlantBloc == generator)
            //    {
            //        return gen.Power;
            //    }
            //}
            //return -1;
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

        //[HttpGet("[action]")]
        private async Task<IEnumerable<PowerDTO>> getData(
                    string documentType = "A73",
                    string processType = "A16",
                    string psrType = "B04",
                    string in_Domain = "10YHU-MAVIR----U",
                    string periodStart = "202209211700",
                    string periodEnd = "202209211800"
                )
        {
            periodStart = getTime(4);
            periodEnd = getTime(3);

            string url = "https://transparency.entsoe.eu/api";
            string securityToken = "a5fb8873-ad26-4972-a5f4-62e2e069f782";
            //string documentType = "A73";
            //string processType = "A16";
            //string psrType = "B14";
            ////string in_Domain = "10YFR-RTE------C";
            ////string periodStart = "202208252000";
            ////string periodEnd = "202208252100";
            //string in_Domain = "10YHU-MAVIR----U";
            //string periodStart = "202209211700";
            //string periodEnd = "202209211800";

            string query = url + "?securityToken=" + securityToken + 
                           "&documentType=" + documentType +
                           "&processType=" + processType + 
                           //"&psrType=" + psrType + 
                           "&in_Domain=" + in_Domain + 
                           "&periodStart=" + periodStart + 
                           "&periodEnd=" + periodEnd;

            var httpClient = new HttpClient();
            
            string apiResponse = "";
            var response = await httpClient.GetAsync(query);

            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;

            apiResponse = await response.Content.ReadAsStringAsync();

            doc.Load(new StringReader(apiResponse));

            //foreach(XmlNode node in doc.ChildNodes[10])
            //System.Diagnostics.Debug.WriteLine("Child Nodes count: " + doc.ChildNodes[2].ChildNodes.Count);

            List<PowerDTO> data = new List<PowerDTO>();
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
                    string name = first.ChildNodes[3].ChildNodes[3].InnerXml;
                    System.Diagnostics.Debug.WriteLine("15: " + name);

                    XmlNode second = node.ChildNodes[17];
                    //System.Diagnostics.Debug.WriteLine("17: " + second.InnerXml);
                    //System.Diagnostics.Debug.WriteLine("17: " + second.ChildNodes.Count);
                    //System.Diagnostics.Debug.WriteLine("17: " + second.ChildNodes[5].InnerXml);
                    //System.Diagnostics.Debug.WriteLine("17: " + second.ChildNodes[5].ChildNodes.Count);
                    //System.Diagnostics.Debug.WriteLine("17: " + second.ChildNodes[5].ChildNodes[3].InnerXml);
                    int power = Int32.Parse(second.ChildNodes[5].ChildNodes[3].InnerXml);
                    System.Diagnostics.Debug.WriteLine("17: " + power);

                    PowerDTO current = new PowerDTO()
                    {
                        PowerPlantBloc = name,
                        Power = power
                    };
                    data.Add(current);
                    sum += power;
                }

                //System.Diagnostics.Debug.WriteLine("Inner node count: " + node.ChildNodes.Count);
                //System.Diagnostics.Debug.WriteLine("I:" + i);
            }

            //System.Diagnostics.Debug.WriteLine(apiResponse);
            System.Diagnostics.Debug.WriteLine(query);
            System.Diagnostics.Debug.WriteLine("Sum: " + sum + ", " + periodEnd);
            System.Diagnostics.Debug.WriteLine("MOST: " + periodStart + ", " + periodEnd);

            data.Add(new PowerDTO() { PowerPlantBloc = "sum" + ", " + periodEnd, Power = sum });
            return data;

            //XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            //nsmgr.AddNamespace("asd", query);
            //string xPathString = "//asd:GL_MarketDocument/asd:TimeSeries/asd:MktPSRType/asd:PowerSystemResources[@name='BELLEVILLE 1']";
            //XmlNode xmlNode = doc.DocumentElement.SelectSingleNode(xPathString, nsmgr);
            //return xmlNode;

            //reservationList = JsonConvert.DeserializeObject<List<Reservation>>(apiResponse);

            //return apiResponse;
        }
    }
}
