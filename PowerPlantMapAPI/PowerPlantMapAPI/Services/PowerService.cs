using PowerPlantMapAPI.Models.DTO;
using System.Data.SqlClient;
using System.Xml;

namespace PowerPlantMapAPI.Services
{
    public class PowerService : IPowerService
    {
        private readonly SqlConnection _connection;
        public PowerService(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            _connection.Open();
        }

        public async Task<CurrentLoadDTO> APIquery(string periodStart, string periodEnd)
        {
            string documentType = "A65";
            string processType = "A16";
            string in_Domain = "10YHU-MAVIR----U";

            string url = "https://transparency.entsoe.eu/api";
            string securityToken = "a5fb8873-ad26-4972-a5f4-62e2e069f782";


            string query = url + "?securityToken=" + securityToken +
                           "&documentType=" + documentType +
                           "&processType=" + processType +
                           "&outBiddingZone_Domain=" + in_Domain +
                           "&periodStart=" + periodStart +
                           "&periodEnd=" + periodEnd;

            var httpClient = new HttpClient();

            string apiResponse = "";
            var response = await httpClient.GetAsync(query);

            apiResponse = await response.Content.ReadAsStringAsync();

            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;

            doc.Load(new StringReader(apiResponse));

            //System.Diagnostics.Debug.WriteLine(doc.ChildNodes[0].ChildNodes.Count);
            //System.Diagnostics.Debug.WriteLine(doc.ChildNodes[1].ChildNodes.Count);
            //System.Diagnostics.Debug.WriteLine(doc.ChildNodes[2].ChildNodes.Count);

            System.Diagnostics.Debug.WriteLine(doc.ChildNodes[2].ChildNodes[19].ChildNodes[3].InnerXml);

            XmlNode Period = doc.ChildNodes[2].ChildNodes[21].ChildNodes[13];

            //for (int i = 5; i < Period.ChildNodes.Count; i+=2)
            //{
            //    //System.Diagnostics.Debug.WriteLine(Period.ChildNodes[i].ChildNodes[3].InnerXml);
            //}

            //System.Diagnostics.Debug.WriteLine(Period.ChildNodes[Period.ChildNodes.Count - 2].ChildNodes[3].InnerXml);

            CurrentLoadDTO load = new CurrentLoadDTO();

            load.end = Period.ChildNodes[1].ChildNodes[3].InnerXml;
            load.CurrentLoad = Int32.Parse(Period.ChildNodes[Period.ChildNodes.Count - 2].ChildNodes[3].InnerXml);
            
            //string end = Period.ChildNodes[1].ChildNodes[3].InnerXml;
            //System.Diagnostics.Debug.WriteLine(end);
            //int a = Int32.Parse(Period.ChildNodes[Period.ChildNodes.Count - 2].ChildNodes[3].InnerXml);
            
            return load;
        }
    }
}
