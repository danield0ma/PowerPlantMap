using Dapper;
using PowerPlantMapAPI.Models.DTO;
using System.Data;
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

        public async Task<IEnumerable<PowerDTO>> getPPData(string docType, string periodStart, string periodEnd)
        {
            //https://transparency.entsoe.eu/api?securityToken=a5fb8873-ad26-4972-a5f4-62e2e069f782&documentType=A73&processType=A16&in_Domain=10YHU-MAVIR----U&periodStart=202210291200&periodEnd=202210291800
            string documentType = docType;
            string processType = "A16";
            string in_Domain = "10YHU-MAVIR----U";
            string url = "https://transparency.entsoe.eu/api";
            string securityToken = "a5fb8873-ad26-4972-a5f4-62e2e069f782";

            string query = url + "?securityToken=" + securityToken +
                           "&documentType=" + documentType +
                           "&processType=" + processType +
                           "&in_Domain=" + in_Domain +
                           "&periodStart=" + periodStart +
                           "&periodEnd=" + periodEnd;

            var httpClient = new HttpClient();

            string apiResponse = "";
            var response = await httpClient.GetAsync(query);

            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;

            apiResponse = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine(apiResponse);
            
            List<PowerDTO> data = new List<PowerDTO>();
            
            try
            {
                doc.Load(new StringReader(apiResponse));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                return data;
            }

            List<int> sum = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                sum.Add(0);
            }

            for (int i = 20; i < doc.ChildNodes[2].ChildNodes.Count; i++)
            {
                XmlNode node = doc.ChildNodes[2].ChildNodes[i];

                if (node.ChildNodes.Count != 0)
                {
                    XmlNode MktPSRType = node.ChildNodes[15];
                    if (docType == "A75")
                    {
                        MktPSRType = node.ChildNodes[13];
                    }

                    string name = "";
                    if (docType == "A73")
                    {
                        name = MktPSRType.ChildNodes[3].ChildNodes[3].InnerXml;
                    }
                    else if (docType == "A75")
                    {
                        name = MktPSRType.ChildNodes[1].InnerXml;
                    }

                    XmlNode Period = node.ChildNodes[15];
                    if (docType == "A73")
                    {
                        Period = node.ChildNodes[17];
                    }

                    List<int> power = new List<int>();
                    for (int j = 5; j < Period.ChildNodes.Count; j += 2)
                    {
                        int p = Int32.Parse(Period.ChildNodes[j].ChildNodes[3].InnerXml);
                        power.Add(p);
                        sum[(j - 5) / 2] += p;
                    }

                    PowerDTO current = new PowerDTO()
                    {
                        PowerPlantBloc = name,
                        Power = power
                    };

                    List<string> codes = new List<string>
                    {
                        "B01", "B04", "B16", "B19"
                    };

                    if (docType == "A73" || (docType == "A75" && codes.Contains(current.PowerPlantBloc)))
                    {
                        data.Add(current);
                    }
                }
            }

            //data.Add(new PowerDTO() { PowerPlantBloc = "sum" + ", " + periodEnd, Power = sum });
            return data;
        }

        public async Task<IEnumerable<PowerDTO>> getImportData(bool export, string periodStart, string periodEnd)
        {
            //https://transparency.entsoe.eu/api?securityToken=a5fb8873-ad26-4972-a5f4-62e2e069f782&documentType=A11&in_Domain=10YHU-MAVIR----U&out_Domain=10YAT-APG------L&periodStart=202110201200&periodEnd=202110201800
            string documentType = "A11";
            string in_Domain = "10YHU-MAVIR----U";
            string url = "https://transparency.entsoe.eu/api";
            string securityToken = "a5fb8873-ad26-4972-a5f4-62e2e069f782";

            List<string> NeighbourCountries = new List<string>
            {
                "10YSK-SEPS-----K", "10YAT-APG------L", "10YSI-ELES-----O",
                "10YHR-HEP------M", "10YCS-SERBIATSOV", "10YRO-TEL------P",
                "10Y1001C--00003F"
            };

            List<PowerDTO> data = new List<PowerDTO>();

            List<int> sum = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                sum.Add(0);
            }

            foreach (string CountryCode in NeighbourCountries)
            {
                string CC = CountryCode;
                if (export)
                {
                    in_Domain = CountryCode;
                    CC = "10YHU-MAVIR----U";
                }

                string end = periodEnd;
                string start = periodStart;

                List<string> problematic = new List<String>
                {
                    "10YSK-SEPS-----K", "10YHR-HEP------M", "10YCS-SERBIATSOV", "10Y1001C--00003F"
                };

                //if (CountryCode == "10YSK-SEPS-----K" || CountryCode == "10YHR-HEP------M" || CountryCode == "10YCS-SERBIATSOV" || CountryCode == "10Y1001C--00003F")
                if (problematic.Contains(CountryCode))
                {
                    start = periodStart.Remove(10);
                    start += "00";
                    end = periodEnd.Remove(10);
                    end += "00";
                }

                string query = url + "?securityToken=" + securityToken +
                                       "&documentType=" + documentType +
                                       "&in_Domain=" + in_Domain +
                                       "&out_Domain=" + CC +
                                       "&periodStart=" + start +
                                       "&periodEnd=" + end;

                var httpClient = new HttpClient();

                string apiResponse = "";
                var response = await httpClient.GetAsync(query);

                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = true;

                apiResponse = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(apiResponse);

                doc.Load(new StringReader(apiResponse));

                XmlNode TimeSeries = doc.ChildNodes[2].ChildNodes[19];
                System.Diagnostics.Debug.Write(TimeSeries);

                int ChildNodeCount;
                try
                {
                    ChildNodeCount = TimeSeries.ChildNodes.Count;
                }
                catch (NullReferenceException e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                    ChildNodeCount = 0;
                }
                
                if (ChildNodeCount != 0)
                {
                    XmlNode Period = TimeSeries.ChildNodes[13];

                    List<int> power = new List<int>();
                    for (int j = 5; j < Period.ChildNodes.Count; j += 2)
                    {
                        int p = Int32.Parse(Period.ChildNodes[j].ChildNodes[3].InnerXml);

                        if(export)
                        {
                            p *= -1;
                        }

                        int n = 1;
                        if (problematic.Contains(CountryCode))
                        {
                            if (j == 5)
                            {
                                if (periodStart.Substring(10, 2) == "15")
                                {
                                    n = 3;
                                }
                                else if (periodStart.Substring(10, 2) == "30")
                                {
                                    n = 2;
                                }
                                else if (periodStart.Substring(10, 2) == "45")
                                {
                                    n = 1;
                                }
                            }
                            
                            else if (j == Period.ChildNodes.Count - 1)
                            {
                                if (periodEnd.Substring(10, 2) == "15")
                                {
                                    n = 1;
                                }
                                if (periodEnd.Substring(10, 2) == "30")
                                {
                                    n = 2;
                                }
                                if (periodEnd.Substring(10, 2) == "45")
                                {
                                    n = 3;
                                }
                            }

                            else
                            {
                                n = 4;
                            }
                        }

                        for (int i = 0; i < n; i++)
                        {
                            power.Add(p);
                            sum[(j - 5) / 2 + i] += p;
                        }
                    }

                    PowerDTO current = new PowerDTO()
                    {
                        PowerPlantBloc = CountryCode,
                        Power = power
                    };

                    data.Add(current);
                }
            }

            //data.Add(new PowerDTO() { PowerPlantBloc = "sum" + ", " + periodStart+ ", " + periodEnd, Power = sum });
            return data;
        }

        public async Task<List<DateTime>> GetStartAndEnd(bool initData)
        {
            DateTime now = DateTime.Now;
            DateTime end = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            DateTime start;

            var parameter = new { PPID = "PKS" };
            List<DateTime> LastData = (List<DateTime>)await _connection.QueryAsync<DateTime>
                ("GetLastDataTime", parameter, commandType: CommandType.StoredProcedure);

            if (!initData)
            {
                //TODO túl régi adat esetén nincs elérhető adat kiírása...
                end = LastData[0];
                start = end.AddDays(-1).AddMinutes(-15);
            }
            else
            {
                if (now.Minute < 15)
                {
                    end = end.AddHours(now.Hour - 1);
                    end = end.AddMinutes(45);
                }
                else if (now.Minute < 30)
                {
                    end = end.AddHours((int)now.Hour);
                    end = end.AddMinutes(0);
                }
                else if (now.Minute < 45)
                {
                    end = end.AddHours(now.Hour);
                    end = end.AddMinutes(15);
                }
                else
                {
                    end = end.AddHours(now.Hour);
                    end = end.AddMinutes(30);
                }
                start = end.AddHours(-30);

                if (LastData[0] > start)
                {
                    start = LastData[0];
                }
            }

            return new List<DateTime> { start, end };
        }

        public async Task<string> InitData(DateTime? periodStart = null, DateTime? periodEnd = null)
        {
            List<DateTime> TimeStamps = new List<DateTime>();
            if (periodStart == null && periodEnd == null)
            {
                TimeStamps = await GetStartAndEnd(true);
            }
            else
            {
                TimeStamps.Add(periodStart.Value);
                TimeStamps.Add(periodEnd.Value);
            }
            DateTime start = TimeStamps[0];

            if ((TimeStamps[1] - TimeStamps[0]).TotalHours <= 24)
            {
                string StartTime = EditTime(TimeStamps[0]);
                string EndTime = EditTime(TimeStamps[1]);

                List<PowerDTO> PowerPlantDataSet = (List<PowerDTO>) await getPPData("A73", StartTime, EndTime);
                List<PowerDTO> RenewableDataSet = (List<PowerDTO>)await getPPData("A75", StartTime, EndTime);
                List<PowerDTO> ImportDataSet = (List<PowerDTO>) await getImportData(false, StartTime, EndTime);
                List<PowerDTO> ExportDataSet = (List<PowerDTO>)await getImportData(true, StartTime, EndTime);

                await saveData(PowerPlantDataSet, start, false);
                await saveData(RenewableDataSet, start, false);
                await saveData(ImportDataSet, start, true);
                await saveData(ExportDataSet, start, false);
            }
            else
            {
                string StartTime = EditTime(TimeStamps[0]);
                string MiddleTime = EditTime(TimeStamps[1].AddHours(-24));
                string EndTime = EditTime(TimeStamps[1]);

                List<PowerDTO> PowerPlantDataSet = (List<PowerDTO>)await getPPData("A73", StartTime, MiddleTime);
                List<PowerDTO> RenewableDataSet = (List<PowerDTO>)await getPPData("A75", StartTime, MiddleTime);
                List<PowerDTO> ImportDataSet = (List<PowerDTO>)await getImportData(false, StartTime, MiddleTime);
                List<PowerDTO> ExportDataSet = (List<PowerDTO>)await getImportData(true, StartTime, MiddleTime);
                
                await saveData(PowerPlantDataSet, start, false);
                await saveData(RenewableDataSet, start, false);
                await saveData(ImportDataSet, start, true);
                await saveData(ExportDataSet, start, false);

                start = TimeStamps[1].AddHours(-24);
                PowerPlantDataSet = (List<PowerDTO>)await getPPData("A73", MiddleTime, EndTime);
                RenewableDataSet = (List<PowerDTO>)await getPPData("A75", MiddleTime, EndTime);
                ImportDataSet = (List<PowerDTO>)await getImportData(false, MiddleTime, EndTime);
                ExportDataSet = (List<PowerDTO>)await getImportData(true, MiddleTime, EndTime);
                
                await saveData(PowerPlantDataSet, start, false);
                await saveData(RenewableDataSet, start, false);
                await saveData(ImportDataSet, start, true);
                await saveData(ExportDataSet, start, false);
            }

            var parameter = new { PPID = "PKS" };
            List<DateTime> LastData = (List<DateTime>)await _connection.QueryAsync<DateTime>
                ("GetLastDataTime", parameter, commandType: CommandType.StoredProcedure);

            return TimeStamps[0] + " - " + TimeStamps[1] + " --> " + LastData[0];
        }

        private async Task<bool> saveData(List<PowerDTO> PowerDataSet, DateTime start, bool import)
        {
            List<string> generators = (List<string>)await _connection.QueryAsync<string>
                    ("GetGenerators", commandType: CommandType.StoredProcedure);

            foreach (PowerDTO PowerData in PowerDataSet)
            {
                if (generators.Contains(PowerData.PowerPlantBloc))
                {
                    DateTime PeriodStart = start.AddMinutes(-15);
                    foreach (int p in PowerData.Power)
                    {
                        PeriodStart = PeriodStart.AddMinutes(15);
                        if (!import || (import && p != 0))
                        {
                            var par = new { GID = PowerData.PowerPlantBloc, start = PeriodStart, end = PeriodStart.AddMinutes(15), power = p };
                            try
                            {
                                await _connection.QueryAsync("AddPastActivity", par, commandType: CommandType.StoredProcedure);
                            }
                            catch (Exception E)
                            {
                                System.Diagnostics.Debug.WriteLine(E.Message);
                                //TODO SQL UPDATE COMMAND kellene!!
                            }
                        }
                        
                    }
                }
            }
            return true;
        }
    }
}

//'Biomass': 'B01',
//'Coal': 'B02',
//'Natural Gas': 'B04',
//'Hard Coal': 'B05',
//'Hydro': 'B11',
//'Hydro Water Reservoir': 'B12',
//'Nuclear': 'B14',
//'Solar': 'B16',
//'Wind': 'B18',
//'Onshore Wind': 'B19'

          //'Albania': '10YAL-KESH-----5',
          //'Austria': '10YAT-APG------L',
          //'Belarus': 'BY',
          //'Belgium': '10YBE----------2',
          //'Bosnia': '10YBA-JPCC-----D',
          //'Bulgaria': '10YCA-BULGARIA-R',
          //'Croatia': '10YHR-HEP------M',
          //'Cyprus': '10YCY-1001A0003J',
          //'Czech': '10YCZ-CEPS-----N',
          //'Denmark': '10Y1001A1001A65H',
          //'Estonia': '10Y1001A1001A39I',
          //'Finland': '10YFI-1--------U',
          //'France': '10YFR-RTE------C',
          //#'Germany': '10Y1001A1001A82H',
          //'Germany': '10Y1001A1001A83F',
          //'Greece': '10YGR-HTSO-----Y',
          //'Hungary': '10YHU-MAVIR----U',
          //'Iceland': 'IS',
          //'Ireland': '10Y1001A1001A59C',
          //'Italy': '10YIT-GRTN-----B',
          //'Latvia': '10YLV-1001A00074',
          //'Lithuania': '10YLT-1001A0008Q',
          //'Louxembourg': '10YLU-CEGEDEL-NQ',
          //'Malta': '10Y1001A1001A93C',
          //'Moldova': '10Y1001A1001A990',
          //'Montenegro': '10YCS-CG-TSO---S',
          //'Netherlands': '10YNL----------L',
          //'Norway': '10YNO-0--------C',
          //'Poland': '10YPL-AREA-----S',
          //'Portugal': '10YPT-REN------W',
          //'Romania': '10YRO-TEL------P',
          //'Serbia': '10YCS-SERBIATSOV',
          //'Slovakia': '10YSK-SEPS-----K',
          //'Slovenia': '10YSI-ELES-----O',
          //'Spain': '10YES-REE------0',
          //'Sweden': '10YSE-1--------K',
          //'Switzerland': '10YCH-SWISSGRIDZ',
          //'Turkey': 'TR',
          //'UK': 'GB',
          //'Ukraine': '10Y1001C--00003F'
