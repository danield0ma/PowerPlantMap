using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using PowerPlantMapAPI.Models;
using PowerPlantMapAPI.Models.DTO;
using PowerPlantMapAPI.Repositories;
using PowerPlantMapAPI.Helpers;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Reflection.Metadata;

namespace PowerPlantMapAPI.Services
{
    public class PowerService : IPowerService
    {
        private readonly IDateService _dateService;
        private readonly IPowerRepository _powerRepository;
        private readonly IPowerHelper _powerHelper;

        public PowerService(IDateService dateService, IPowerRepository repository, IPowerHelper powerHelper)
        {
            _dateService = dateService;
            _powerRepository = repository;
            _powerHelper = powerHelper;
        }

        public async Task<ActionResult<IEnumerable<FeatureDTO>>> GetPowerPlantBasics()
        {
            List<PowerPlantDataModel> PowerPlants = await _powerRepository.QueryPowerPlantBasics();

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
            return await _powerRepository.QueryBasicsOfPowerPlant(id);
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

            List<PowerPlantDetailsModel> PowerPlantDetails = await _powerRepository.QueryPowerPlantDetails(Id);

            List<DateTime> TimeStamps = await _dateService.HandleWhichDateFormatIsBeingUsed(Date, Start, End);
            PowerPlant.DataStart = TimeStamps[0];
            PowerPlant.DataEnd = TimeStamps[1];

            if (Date != null)
            {
                string msg = await CheckWhetherDataIsPresentInTheGivenTimePeriod(TimeStamps);
                System.Diagnostics.Debug.WriteLine(msg);
            }

            PowerPlant.DataStart = TimeStamps[0];
            PowerPlant.DataEnd = TimeStamps[1];

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
                    Generator.PastPower = await _powerHelper.GetGeneratorPower(Generator.GeneratorID, TimeStamps[0], TimeStamps[1]);
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
            List<string> PowerPlants = await _powerRepository.QueryPowerPlants();

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
                TimeStamps.Add(TimeZoneInfo.ConvertTimeToUtc(PeriodStart.Value, TimeZoneInfo.Local));
                TimeStamps.Add(TimeZoneInfo.ConvertTimeToUtc(PeriodEnd.Value, TimeZoneInfo.Local));
            }
            else
            {
                TimeStamps = await _dateService.GetInitDataTimeInterval();
            }

            if ((TimeStamps[1] - TimeStamps[0]).TotalHours <= 24)
            {
                string StartTime = _dateService.EditTime(TimeStamps[0]);
                string EndTime = _dateService.EditTime(TimeStamps[1]);

                List<PowerOfPowerPlantDTO> PowerPlantDataSet = (List<PowerOfPowerPlantDTO>)await GetPPData("A73", StartTime, EndTime);
                List<PowerOfPowerPlantDTO> RenewableDataSet = (List<PowerOfPowerPlantDTO>)await GetPPData("A75", StartTime, EndTime);
                List<PowerOfPowerPlantDTO> ImportDataSet = (List<PowerOfPowerPlantDTO>)await GetImportData(false, StartTime, EndTime);
                List<PowerOfPowerPlantDTO> ExportDataSet = (List<PowerOfPowerPlantDTO>)await GetImportData(true, StartTime, EndTime);

                await SaveQueriedDataToDb(PowerPlantDataSet, TimeStamps[0], false);
                await SaveQueriedDataToDb(RenewableDataSet, TimeStamps[0], false);
                await SaveQueriedDataToDb(ImportDataSet, TimeStamps[0], true);
                await SaveQueriedDataToDb(ExportDataSet, TimeStamps[0], false);
            }
            else
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
            }

            List<DateTime> LastData = await _powerRepository.QueryLastDataTime();
            return TimeStamps[0] + " - " + TimeStamps[1] + " --> " + LastData[0];
        }

        private async Task<IEnumerable<PowerOfPowerPlantDTO>> GetPPData(string DocType, string PeriodStart, string PeriodEnd)
        {
            List<PowerOfPowerPlantDTO> PPData = new List<PowerOfPowerPlantDTO>();

            List<string> codes = new List<string>
            {
                "B01", "B04", "B16", "B19"
            };

            try
            {
                XDocument document = XDocument.Parse(await _powerHelper.APIquery(DocType, PeriodStart, PeriodEnd));
                XNamespace ns = document?.Root.Name.Namespace;
                List<int> Sum = Enumerable.Repeat(0, 100).ToList();

                if (document is not null && document?.Root is not null && document?.Root?.Elements(ns + "TimeSeries") is not null)
                {
                    //foreach (var element in document?.Root?.Elements())
                    //{
                    //    Console.WriteLine("Element: " + element.Name);
                    //}

                    foreach (var TimeSeries in document?.Root?.Elements(ns + "TimeSeries"))
                    {
                        string? name = "";
                        if (DocType == "A73")
                        {
                            name = TimeSeries?.Element(ns + "MktPSRType")?.Element(ns + "PowerSystemResources")?.Element(ns + "name")?.Value;
                        }
                        if (DocType == "A75")
                        {
                            name = TimeSeries?.Element(ns + "MktPSRType")?.Element(ns + "psrType")?.Value;
                        }

                        if (DocType == "A73" || DocType == "A75" && name is not null && codes.Contains(name))
                        {
                            XElement Period = TimeSeries.Element(ns + "Period");
                            List<int> power = new List<int>();
                            if (Period is not null)
                            {
                                foreach (XElement Point in Period.Elements(ns + "Point"))
                                {
                                    int currentPower = Convert.ToInt32(Point?.Element(ns + "quantity")?.Value);
                                    power.Add(currentPower);
                                    Sum[Convert.ToInt32(Point?.Element(ns + "position")?.Value)] += currentPower;
                                }
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
                            
                            PPData.Add(current);
                        }
                    }
                }
            }
            catch (Exception Exception)
            {
                Console.WriteLine(Exception);
            }

            return PPData;
        }

        private async Task<IEnumerable<PowerOfPowerPlantDTO>> GetImportData(bool export, string periodStart, string periodEnd)
        {
            List<string> neighbourCountries = new List<string>
            {
                "10YSK-SEPS-----K",
                "10YAT-APG------L",
                "10YSI-ELES-----O",
                "10YHR-HEP------M",
                "10YCS-SERBIATSOV",
                "10YRO-TEL------P",
                "10Y1001C--00003F"
            };

            List<string> problematic = new List<string>
            {
                "10YSK-SEPS-----K",
                "10YHR-HEP------M",
                "10YCS-SERBIATSOV",
                "10Y1001C--00003F"
            };

            List<PowerOfPowerPlantDTO> importData = new List<PowerOfPowerPlantDTO>();
            List<int> sum = Enumerable.Repeat(0, 100).ToList();

            foreach (string countryCode in neighbourCountries)
            {
                if (problematic.Contains(countryCode))
                {
                    periodStart = periodStart.Remove(10);
                    periodStart += "00";
                    periodEnd = periodEnd.Remove(10);
                    periodEnd += "00";
                }

                try
                {
                    XDocument document;
                    if (export)
                    {
                        document = XDocument.Parse(await _powerHelper.APIquery("A11", periodStart, periodEnd, "10YHU-MAVIR----U", countryCode));
                    }
                    else
                    {
                        document = XDocument.Parse(await _powerHelper.APIquery("A11", periodStart, periodEnd, countryCode, "10YHU-MAVIR----U"));
                    }

                    XNamespace ns = document.Root.Name.Namespace;
                    if (document is not null && document?.Root is not null && document?.Root?.Elements(ns + "TimeSeries") is not null)
                    {
                        var TimeSeries = document?.Root?.Element(ns + "TimeSeries");
                        if (TimeSeries?.Elements() is not null)
                        {
                            var Period = TimeSeries?.Element(ns + "Period");
                            List<PowerStampDTO> powerStamps = new List<PowerStampDTO>();

                            int index = 0;
                            if (Period?.Elements(ns + "Point") is not null)
                            {
                                foreach (var Point in Period.Elements(ns + "Point"))
                                {
                                    PowerStampDTO powerStamp = new PowerStampDTO();
                                    int currentPower = Convert.ToInt32(Point?.Element(ns + "quantity")?.Value);
                                    if (export)
                                    {
                                        currentPower *= -1;
                                    }

                                    int numberOfTimesTheValueHasToBeSaved = 1;
                                    if (problematic.Contains(countryCode))
                                    {
                                        numberOfTimesTheValueHasToBeSaved = 4;
                                    }

                                    powerStamp.Start = DateTime.Now;
                                    powerStamp.Power = currentPower;

                                    for (int i = 0; i < numberOfTimesTheValueHasToBeSaved; i++)
                                    {
                                        powerStamps.Add(powerStamp);
                                        sum[Convert.ToInt32(Point?.Element(ns + "position")?.Value)] += currentPower;
                                    }

                                    index++;
                                }
                            }

                            PowerOfPowerPlantDTO current = new PowerOfPowerPlantDTO()
                            {
                                PowerPlantName = countryCode,
                                PowerStamps = powerStamps
                            };

                            importData.Add(current);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return importData;
        }

        private async Task<bool> SaveQueriedDataToDb(List<PowerOfPowerPlantDTO> PowerDataSet, DateTime Start, bool Import)
        {
            List<string> Generators = await _powerRepository.QueryGenerators();

            foreach (PowerOfPowerPlantDTO PowerData in PowerDataSet)
            {
                if (PowerData.PowerPlantName != null && Generators.Contains(PowerData.PowerPlantName))
                {
                    DateTime PeriodStart = Start.AddMinutes(-15);
                    foreach (PowerStampDTO PowerStamp in PowerData.PowerStamps)
                    {
                        PeriodStart = PeriodStart.AddMinutes(15);
                        if (!Import || (Import && PowerStamp.Power != 0))
                        //TODO mi ez a feltétel?
                        {
                            await _powerRepository.InsertData(PowerData.PowerPlantName, PeriodStart, PowerStamp.Power);
                        }
                    }
                }
            }
            return true;
        }

        private async Task<string> CheckWhetherDataIsPresentInTheGivenTimePeriod(List<DateTime> TimeStamps)
        {
            List<PastActivityModel> PastActivity = await _powerRepository.QueryPastActivity("PA_gép1", TimeStamps[0], TimeStamps[1]);

            if (PastActivity.Count < 10)
            {
                return await InitData(TimeStamps[0].AddHours(-2), TimeStamps[1].AddHours(2));
            }
            return "no InitData";
        }
    }
}
