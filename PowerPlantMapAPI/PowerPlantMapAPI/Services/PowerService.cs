﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using PowerPlantMapAPI.Models;
using PowerPlantMapAPI.Models.DTO;
using PowerPlantMapAPI.Repositories;
using PowerPlantMapAPI.Helpers;
using System.Collections.Generic;
using System.Xml;

namespace PowerPlantMapAPI.Services
{
    public class PowerService : IPowerService
    {
        private readonly IDateService _dateService;
        private readonly IPowerRepository _repository;
        private readonly IPowerHelper _powerHelper;

        public PowerService(IDateService dateService, IPowerRepository repository, IPowerHelper powerHelper)
        {
            _dateService = dateService;
            _repository = repository;
            _powerHelper = powerHelper;
        }

        public async Task<ActionResult<IEnumerable<FeatureDTO>>> GetPowerPlantBasics()
        {
            List<PowerPlantDataModel> PowerPlants = await _repository.QueryPowerPlantBasics();

            List<FeatureDTO> PowerPlantBasics = new List<FeatureDTO>();

            foreach (var PowerPlant in PowerPlants)
            {
                FeatureDTO feature = new FeatureDTO();

                FeaturePropertyDTO properties = new FeaturePropertyDTO();
                properties.id = PowerPlant.PowerPlantID;
                properties.name = PowerPlant.name;
                properties.description = PowerPlant.description;
                properties.img = PowerPlant.image;
                feature.properties = properties;

                FeatureGeometryDTO geometry = new FeatureGeometryDTO();
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

        public async Task<PowerPlantDataModel> GetBasicsOfPowerPlant(string id)
        {
            return await _repository.QueryBasicsOfPowerPlant(id);
        }

        public async Task<ActionResult<PowerPlantDetailsDTO>> GetDetailsOfPowerPlant(string Id, DateTime? Date = null, DateTime? Start = null, DateTime? End = null)
        {
            PowerPlantDetailsDTO PowerPlant = new PowerPlantDetailsDTO();
            PowerPlant.PowerPlantID = Id;
            PowerPlantDataModel BasicsOfPowerPlant = await GetBasicsOfPowerPlant(Id);

            PowerPlant.name = BasicsOfPowerPlant.name;
            PowerPlant.description = BasicsOfPowerPlant.description;
            PowerPlant.OperatorCompany = BasicsOfPowerPlant.OperatorCompany;
            PowerPlant.webpage = BasicsOfPowerPlant.webpage;
            PowerPlant.Color = BasicsOfPowerPlant.color;
            PowerPlant.Address = BasicsOfPowerPlant.address;
            PowerPlant.IsCountry = BasicsOfPowerPlant.IsCountry;
            PowerPlant.longitude = Math.Round(BasicsOfPowerPlant.longitude, 4);
            PowerPlant.latitude = Math.Round(BasicsOfPowerPlant.latitude, 4);

            List<PowerPlantDetailsModel> PowerPlantDetails = await _repository.QueryPowerPlantDetails(Id);

            List<DateTime> TimeStamps = await _dateService.HandleWhichDateFormatIsBeingUsed(Date, Start, End);
            PowerPlant.DataStart = TimeStamps[0];
            PowerPlant.DataEnd = TimeStamps[1];

            if (Date != null)
            {
                string msg = await CheckWhetherDataIsPresentInTheGivenTimePeriod(TimeStamps);
                System.Diagnostics.Debug.WriteLine(msg);
            }

            int PPMaxPower = 0, PPCurrentPower = 0, i = 0;
            List<BlocDTO> Blocs = new List<BlocDTO>();
            while (i < PowerPlantDetails.Count)
            {
                BlocDTO Bloc = new BlocDTO();
                Bloc.BlocID = PowerPlantDetails[i].BlocId;
                Bloc.BlocType = PowerPlantDetails[i].BlocType;
                Bloc.MaxBlocCapacity = PowerPlantDetails[i].MaxBlocCapacity;
                Bloc.ComissionDate = PowerPlantDetails[i].ComissionDate;

                List<GeneratorDTO> Generators = new List<GeneratorDTO>();
                int CurrentPower = 0, MaxPower = 0;
                while (i < PowerPlantDetails.Count && PowerPlantDetails[i].BlocId == Bloc.BlocID)
                {
                    GeneratorDTO Generator = new GeneratorDTO();
                    Generator.GeneratorID = PowerPlantDetails[i].GeneratorID;
                    Generator.MaxCapacity = PowerPlantDetails[i].MaxCapacity;
                    Generator.PastPower = await _powerHelper.GetGeneratorPower(Generator.GeneratorID, PowerPlant.DataStart, PowerPlant.DataEnd);
                    Generators.Add(Generator);
                    CurrentPower += Generator.PastPower[Generator.PastPower.Count - 1].Power;
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

        public async Task<IEnumerable<PowerStampDTO>> GetPowerOfPowerPlant(string Id, DateTime? Date = null, DateTime? Start = null, DateTime? End = null)
        {
            List<DateTime> TimeStamps = await _dateService.HandleWhichDateFormatIsBeingUsed(Date, Start, End);
            int NumberOfDataPoints = _dateService.CalculateTheNumberOfIntervals(TimeStamps[0], TimeStamps[1]);
            List<PowerStampDTO> PowerStamps = await _powerHelper.GetPowerStampsListOfPowerPlant(Id, NumberOfDataPoints, TimeStamps);
            return PowerStamps;
        }

        public async Task<PowerOfPowerPlantsDTO> GetPowerOfPowerPlants(DateTime? Date = null, DateTime? Start = null, DateTime? End = null)
        {
            PowerOfPowerPlantsDTO PowerOfPowerPlants = new PowerOfPowerPlantsDTO();
            List<string> PowerPlants = await _repository.QueryPowerPlants();

            List<DateTime> TimeStamps = await _dateService.HandleWhichDateFormatIsBeingUsed(Date, Start, End);
            PowerOfPowerPlants.Start = TimeStamps[0];
            PowerOfPowerPlants.End = TimeStamps[1];

            if (Date != null)
            {
                string msg = await CheckWhetherDataIsPresentInTheGivenTimePeriod(TimeStamps);
                System.Diagnostics.Debug.WriteLine(msg);
            }

            int NumberOfDataPoints = _dateService.CalculateTheNumberOfIntervals(PowerOfPowerPlants.Start, PowerOfPowerPlants.End);

            List<PowerOfPowerPlantDTO> Data = new List<PowerOfPowerPlantDTO>();

            foreach (string PowerPlant in PowerPlants)
            {
                PowerOfPowerPlantDTO PowerOfPowerPlant = new PowerOfPowerPlantDTO();
                PowerOfPowerPlant.PowerPlantName = PowerPlant;
                PowerOfPowerPlant.PowerStamps = await _powerHelper.GetPowerStampsListOfPowerPlant(PowerPlant, NumberOfDataPoints, TimeStamps);
                Data.Add(PowerOfPowerPlant);
            }

            PowerOfPowerPlants.Data = Data;
            return PowerOfPowerPlants;
        }

        public async Task<string> InitData(DateTime? PeriodStart = null, DateTime? PeriodEnd = null)
        {
            List<DateTime> TimeStamps = new List<DateTime>();
            if (PeriodStart is DateTime && PeriodEnd is DateTime)
            {
                TimeStamps.Add(PeriodStart.Value);
                TimeStamps.Add(PeriodEnd.Value);
            }
            else
            {
                TimeStamps = await _dateService.GetInitDataTimeInterval();
            }

            if ((TimeStamps[1] - TimeStamps[0]).TotalHours <= 24)
            {
                string StartTime = _dateService.EditTime(TimeStamps[0]);
                string EndTime = _dateService.EditTime(TimeStamps[1]);

                List<PowerOfPowerPlantDTO> PowerPlantDataSet = (List<PowerOfPowerPlantDTO>)await getPPData("A73", StartTime, EndTime);
                List<PowerOfPowerPlantDTO> RenewableDataSet = (List<PowerOfPowerPlantDTO>)await getPPData("A75", StartTime, EndTime);
                List<PowerOfPowerPlantDTO> ImportDataSet = (List<PowerOfPowerPlantDTO>)await getImportData(false, StartTime, EndTime);
                List<PowerOfPowerPlantDTO> ExportDataSet = (List<PowerOfPowerPlantDTO>)await getImportData(true, StartTime, EndTime);

                await saveData(PowerPlantDataSet, TimeStamps[0], false);
                await saveData(RenewableDataSet, TimeStamps[0], false);
                await saveData(ImportDataSet, TimeStamps[0], true);
                await saveData(ExportDataSet, TimeStamps[0], false);
            }
            else// if ((TimeStamps[1] - TimeStamps[0]).TotalHours <= 48)
            {
                DateTime CurrentTime = TimeStamps[0];
                while (CurrentTime < TimeStamps[1])
                {
                    DateTime End = CurrentTime.AddHours(24);
                    if (End > TimeStamps[1])
                    {
                        End = TimeStamps[1];
                    }

                    await InitData(CurrentTime, End);
                    CurrentTime = CurrentTime.AddHours(24);
                }

                //string Response = await InitData(TimeStamps[0], TimeStamps[0].AddHours(24));
                //Response = await InitData(TimeStamps[0].AddHours(24), TimeStamps[1]);
            }
            //else
            //{
            //    throw new NotImplementedException("Time interval is larger than 48 hours.");
            //}

            List<DateTime> LastData = await _repository.QueryLastDataTime();
            return TimeStamps[0] + " - " + TimeStamps[1] + " --> " + LastData[0];
        }

        private async Task<IEnumerable<PowerOfPowerPlantDTO>> getPPData(string docType, string periodStart, string periodEnd)
        {
            List<PowerOfPowerPlantDTO> data = new List<PowerOfPowerPlantDTO>();

            XmlDocument doc;
            try
            {
                doc = await _powerHelper.APIquery(docType, periodStart, periodEnd);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                return data;
            }

            //string documentType = docType;
            //string processType = "A16";
            //string in_Domain = "10YHU-MAVIR----U";
            //string url = "https://web-api.tp.entsoe.eu/api";
            //string securityToken = "a5fb8873-ad26-4972-a5f4-62e2e069f782";

            //string query = url + "?securityToken=" + securityToken +
            //               "&documentType=" + documentType +
            //               "&processType=" + processType +
            //               "&in_Domain=" + in_Domain +
            //               "&periodStart=" + periodStart +
            //               "&periodEnd=" + periodEnd;

            //var httpClient = new HttpClient();

            //string apiResponse = "";
            //var response = await httpClient.GetAsync(query);

            //XmlDocument doc = new XmlDocument();
            //doc.PreserveWhitespace = true;

            //apiResponse = await response.Content.ReadAsStringAsync();
            ////System.Diagnostics.Debug.WriteLine(apiResponse);

            //try
            //{
            //    doc.Load(new StringReader(apiResponse));
            //}
            //catch (Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine(e);
            //    return data;
            //}

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

                    List<PowerStampDTO> PowerStamps = new List<PowerStampDTO>();
                    foreach (int p in power)
                    {
                        PowerStampDTO PowerStamp = new PowerStampDTO();
                        PowerStamp.Power = p;
                        PowerStamp.Start = DateTime.Now;
                        PowerStamps.Add(PowerStamp);
                    }

                    PowerOfPowerPlantDTO current = new PowerOfPowerPlantDTO()
                    {
                        PowerPlantName = name,
                        PowerStamps = PowerStamps
                    };

                    List<string> codes = new List<string>
                    {
                        "B01", "B04", "B16", "B19"
                    };

                    if (docType == "A73" || (docType == "A75" && codes.Contains(current.PowerPlantName)))
                    {
                        data.Add(current);
                    }
                }
            }

            //data.Add(new PowerDTO() { PowerPlantBloc = "sum" + ", " + periodEnd, Power = sum });
            return data;
        }

        private async Task<IEnumerable<PowerOfPowerPlantDTO>> getImportData(bool export, string periodStart, string periodEnd)
        {
            string documentType = "A11";
            string in_Domain = "10YHU-MAVIR----U";
            string url = "https://web-api.tp.entsoe.eu/api";
            string securityToken = "a5fb8873-ad26-4972-a5f4-62e2e069f782";

            List<string> NeighbourCountries = new List<string>
            {
                "10YSK-SEPS-----K",
                "10YAT-APG------L",
                "10YSI-ELES-----O",
                "10YHR-HEP------M",
                "10YCS-SERBIATSOV",
                "10YRO-TEL------P",
                "10Y1001C--00003F"
            };

            List<PowerOfPowerPlantDTO> data = new List<PowerOfPowerPlantDTO>();

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
                    "10YSK-SEPS-----K",
                    "10YHR-HEP------M",
                    "10YCS-SERBIATSOV",
                    "10Y1001C--00003F"
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

                    List<PowerStampDTO> PowerStamps = new List<PowerStampDTO>();
                    for (int j = 5; j < Period.ChildNodes.Count; j += 2)
                    {
                        PowerStampDTO PowerStamp = new PowerStampDTO();
                        int p = Int32.Parse(Period.ChildNodes[j].ChildNodes[3].InnerXml);

                        if (export)
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

                        PowerStamp.Start = DateTime.Now;
                        PowerStamp.Power = p;

                        for (int i = 0; i < n; i++)
                        {
                            PowerStamps.Add(PowerStamp);
                            sum[(j - 5) / 2 + i] += p;
                        }
                    }

                    PowerOfPowerPlantDTO current = new PowerOfPowerPlantDTO()
                    {
                        PowerPlantName = CountryCode,
                        PowerStamps = PowerStamps
                    };

                    data.Add(current);
                }
            }

            //data.Add(new PowerDTO() { PowerPlantBloc = "sum" + ", " + periodStart+ ", " + periodEnd, Power = sum });
            return data;
        }

        private async Task<bool> saveData(List<PowerOfPowerPlantDTO> PowerDataSet, DateTime start, bool import)
        {
            List<string> generators = await _repository.QueryGenerators();

            foreach (PowerOfPowerPlantDTO PowerData in PowerDataSet)
            {
                if (generators.Contains(PowerData.PowerPlantName))
                {
                    DateTime PeriodStart = start.AddMinutes(-15);
                    foreach (PowerStampDTO PowerStamp in PowerData.PowerStamps)
                    {
                        PeriodStart = PeriodStart.AddMinutes(15);
                        if (!import || (import && PowerStamp.Power != 0))
                        {
                            await _repository.InsertData(PowerData.PowerPlantName, PeriodStart, PowerStamp.Power);
                        }

                    }
                }
            }
            return true;
        }

        private async Task<string> CheckWhetherDataIsPresentInTheGivenTimePeriod(List<DateTime> TimeStamps)
        {
            List<PastActivityModel> PastActivity = await _repository.QueryPastActivity("PA_gép1", TimeStamps[0], TimeStamps[1]);

            if (PastActivity.Count < 10)
            {
                return await InitData(TimeStamps[0].AddHours(-2), TimeStamps[1].AddHours(2));
            }
            return "no InitData";
        }
    }
}
