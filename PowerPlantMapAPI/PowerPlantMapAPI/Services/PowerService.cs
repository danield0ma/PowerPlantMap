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

        private async Task<XmlNode> APIquery(string periodStart, string periodEnd)
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

            return Period;
        }

        private DateTime TransformTime(string time)
        {
            DateTime t = new DateTime(Int32.Parse(time.Substring(0, 4)),
                Int32.Parse(time.Substring(5, 2)), Int32.Parse(time.Substring(8, 2)),
                Int32.Parse(time.Substring(11, 2)), Int32.Parse(time.Substring(14, 2)), 00);

            return t;
        }

        public async Task<CurrentLoadDTO> GetCurrentLoad(string periodStart, string periodEnd)
        {
            XmlNode Period = await APIquery(periodStart, periodEnd);

            CurrentLoadDTO load = new CurrentLoadDTO();

            load.end = TransformTime(Period.ChildNodes[1].ChildNodes[3].InnerXml);
            load.CurrentLoad = Int32.Parse(Period.ChildNodes[Period.ChildNodes.Count - 2].ChildNodes[3].InnerXml);
            
            return load;
        }
        
        public async Task<IEnumerable<CurrentLoadDTO>> GetLoadHistory(DateTime periodStart, DateTime periodEnd)
        {
            XmlNode Period = await APIquery(EditTime(periodStart), EditTime(periodEnd));

            List<CurrentLoadDTO> loadHistory = new List<CurrentLoadDTO>();

            for (int i = 5; i < Period.ChildNodes.Count; i+=2)
            {
                CurrentLoadDTO load = new CurrentLoadDTO();
                load.CurrentLoad = Int32.Parse(Period.ChildNodes[i].ChildNodes[3].InnerXml);
                periodStart = periodStart.AddMinutes(15);
                load.end = periodStart;
                loadHistory.Add(load);
            }

            return loadHistory;
        }

        public string EditTime(DateTime start)
        {
            string StartTime = Convert.ToString(start.Year);
            if (start.Month < 10) { StartTime += "0"; }
            StartTime += Convert.ToString(start.Month);
            if (start.Day < 10) { StartTime += "0"; }
            StartTime += Convert.ToString(start.Day);
            if (start.Hour < 10) { StartTime += "0"; }
            StartTime += Convert.ToString(start.Hour);
            if (start.Minute < 10) { StartTime += "0"; }
            StartTime += Convert.ToString(start.Minute);
            return StartTime;
        }
    }
}
