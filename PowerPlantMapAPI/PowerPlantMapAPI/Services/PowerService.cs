using Microsoft.AspNetCore.Mvc;
using PowerPlantMapAPI.Models;
using PowerPlantMapAPI.Models.DTO;
using PowerPlantMapAPI.Repositories;
using System.Data.SqlClient;
using System.Xml;

namespace PowerPlantMapAPI.Services
{
    public class PowerService : IPowerService
    {
        private readonly IDateService _dateService;
        private readonly IPowerRepository _repository;
        
        public PowerService(IDateService dateService, IPowerRepository repository)
        {
            _dateService = dateService;
            _repository = repository;
        }

        public async Task<ActionResult<IEnumerable<FeatureModel>>> getPowerPlantBasics()
        {
            List<PowerPlantDataDTO> PowerPlants = await _repository.QueryPowerPlantBasics();

            List <FeatureModel> PowerPlantBasics = new List<FeatureModel>();

            foreach (var PowerPlant in PowerPlants)
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

        public async Task<BasicsOfPowerPlantDTO> GetBasicsOfPowerPlant(string id)
        {
            List<PowerPlantDataDTO> PP = await _repository.QueryBasicsOfPowerPlant(id);

            PowerPlantDataDTO PowerPlant = PP[0];
            BasicsOfPowerPlantDTO b = new BasicsOfPowerPlantDTO();
            b.id = PowerPlant.PowerPlantID;
            b.name = PowerPlant.name;
            b.description = PowerPlant.description;
            b.OperatorCompany = PowerPlant.OperatorCompany;
            b.webpage = PowerPlant.webpage;
            b.Color = PowerPlant.Color;
            b.Address = PowerPlant.Address;
            b.IsCountry = PowerPlant.IsCountry;
            b.longitude = PowerPlant.longitude;
            b.latitude = PowerPlant.latitude;

            return b;
        }

        public async Task<ActionResult<PowerPlantDetailsModel>> getDetailsOfPowerPlant(string id, DateTime? date = null)
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
            PowerPlant.IsCountry = basics.IsCountry;
            PowerPlant.longitude = Math.Round(basics.longitude, 4);
            PowerPlant.latitude = Math.Round(basics.latitude, 4);

            List<PowerPlantDetailsDTO> PowerPlantDetails = await _repository.QueryPowerPlantDetails(id);

            List <DateTime> TimeStamps = await _dateService.CheckDate(date);
            
            if (date != null)
            {
                string msg = await CheckData(TimeStamps);
                System.Diagnostics.Debug.WriteLine(msg);
            }

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

        public async Task<PowerOfPowerPlantsModel> GetPowerOfPowerPlants(DateTime? date = null)
        {
            PowerOfPowerPlantsModel P = new PowerOfPowerPlantsModel();
            List<string> PowerPlants = await _repository.QueryPowerPlants();

            List<DateTime> TimeStamps = await _dateService.CheckDate(date);
            P.Start = TimeStamps[0];
            P.End = TimeStamps[1];

            if (date != null)
            {
                string msg = await CheckData(TimeStamps);
                System.Diagnostics.Debug.WriteLine(msg);
            }

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

                List<string> Generators = await _repository.QueryGeneratorsOfPowerPlant(PowerPlant);

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

            P.Data = PowerOfPPs;
            return P;
        }

        public async Task<string> InitData(DateTime? periodStart = null, DateTime? periodEnd = null)
        {
            List<DateTime> TimeStamps = new List<DateTime>();
            if (periodStart == null && periodEnd == null)
            {
                TimeStamps = await _dateService.GetStartAndEnd(true);
            }
            else
            {
                TimeStamps.Add(periodStart.Value);
                TimeStamps.Add(periodEnd.Value);
            }
            DateTime start = TimeStamps[0];

            if ((TimeStamps[1] - TimeStamps[0]).TotalHours <= 24)
            {
                string StartTime = _dateService.EditTime(TimeStamps[0]);
                string EndTime = _dateService.EditTime(TimeStamps[1]);

                List<PowerDTO> PowerPlantDataSet = (List<PowerDTO>)await getPPData("A73", StartTime, EndTime);
                List<PowerDTO> RenewableDataSet = (List<PowerDTO>)await getPPData("A75", StartTime, EndTime);
                List<PowerDTO> ImportDataSet = (List<PowerDTO>)await getImportData(false, StartTime, EndTime);
                List<PowerDTO> ExportDataSet = (List<PowerDTO>)await getImportData(true, StartTime, EndTime);

                await saveData(PowerPlantDataSet, start, false);
                await saveData(RenewableDataSet, start, false);
                await saveData(ImportDataSet, start, true);
                await saveData(ExportDataSet, start, false);
            }
            else
            {
                string StartTime = _dateService.EditTime(TimeStamps[0]);
                string MiddleTime = _dateService.EditTime(TimeStamps[1].AddHours(-24));
                string EndTime = _dateService.EditTime(TimeStamps[1]);

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

            //var parameter = new { PPID = "PKS" };
            //List<DateTime> LastData = (List<DateTime>)await _connection.QueryAsync<DateTime>
            //    ("GetLastDataTime", parameter, commandType: CommandType.StoredProcedure);
            List<DateTime> LastData = await _repository.QueryLastDataTime();

            return TimeStamps[0] + " - " + TimeStamps[1] + " --> " + LastData[0];
        }

        public async Task<CurrentLoadDTO> GetCurrentLoad(string periodStart, string periodEnd)
        {
            XmlNode Period = await APIquery(periodStart, periodEnd);

            CurrentLoadDTO load = new CurrentLoadDTO();

            load.end = _dateService.TransformTime(Period.ChildNodes[1].ChildNodes[3].InnerXml);
            load.CurrentLoad = Int32.Parse(Period.ChildNodes[Period.ChildNodes.Count - 2].ChildNodes[3].InnerXml);

            return load;
        }

        public async Task<IEnumerable<CurrentLoadDTO>> GetLoadHistory(DateTime periodStart, DateTime periodEnd)
        {
            XmlNode Period = await APIquery(_dateService.EditTime(periodStart), _dateService.EditTime(periodEnd));

            List<CurrentLoadDTO> loadHistory = new List<CurrentLoadDTO>();

            for (int i = 5; i < Period.ChildNodes.Count; i += 2)
            {
                CurrentLoadDTO load = new CurrentLoadDTO();
                load.CurrentLoad = Int32.Parse(Period.ChildNodes[i].ChildNodes[3].InnerXml);
                periodStart = periodStart.AddMinutes(15);
                load.end = periodStart;
                loadHistory.Add(load);
            }

            return loadHistory;
        }

        private async Task<IEnumerable<PowerDTO>> getPPData(string docType, string periodStart, string periodEnd)
        {
            string documentType = docType;
            string processType = "A16";
            string in_Domain = "10YHU-MAVIR----U";
            string url = "https://web-api.tp.entsoe.eu/api";
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

        private async Task<IEnumerable<PowerDTO>> getImportData(bool export, string periodStart, string periodEnd)
        {
            string documentType = "A11";
            string in_Domain = "10YHU-MAVIR----U";
            string url = "https://web-api.tp.entsoe.eu/api";
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
                                if (periodStart.Substring(10, 2) == "00")
                                {
                                    n = 4;
                                }
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

        private async Task<bool> saveData(List<PowerDTO> PowerDataSet, DateTime start, bool import)
        {
            List<string> generators = await _repository.QueryGenerators();

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
                            await _repository.InsertData(PowerData.PowerPlantBloc, PeriodStart, p);
                        }
                        
                    }
                }
            }
            return true;
        }

        private async Task<List<int>> GetGeneratorPower(string generator, DateTime start, DateTime end)
        //TODO GeneratorPowerDTO-val térjen vissza, hogy ne a frontenden kelljen az időket hozzáigazítani
        {
            List<PastActivityModel> PastActivity = await _repository.QueryPastActivity(generator, start, end);

            List<int> power = new List<int>();
            foreach (var Activity in PastActivity)
            {
                power.Add(Activity.ActualPower);
            }

            for (int i = power.Count; i < 97; i++)
            {
                power.Add(0);
            }
            return power;
        }

        private async Task<string> CheckData(List<DateTime> TimeStamps)
        {
            List<PastActivityModel> PastActivity = await _repository.QueryPastActivity("PA_gép1", TimeStamps[0], TimeStamps[1]);

            if (PastActivity.Count < 10)
            {
                return await InitData(TimeStamps[0].AddHours(-2), TimeStamps[1].AddHours(2));
            }
            return "no InitData";
        }

        private async Task<XmlNode> APIquery(string periodStart, string periodEnd)
        {
            string documentType = "A65";
            string processType = "A16";
            string in_Domain = "10YHU-MAVIR----U";

            string url = "https://web-api.tp.entsoe.eu/api";
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
