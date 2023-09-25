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
using System.Threading.Tasks;
using System.Drawing;

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

        public async Task<ActionResult<PowerPlantDetailsDTO>> GetDetailsOfPowerPlant(string id, DateTime? date = null, DateTime? startLocal = null, DateTime? endLocal = null)
        {
            PowerPlantDetailsDTO PowerPlant = new()
            {
                PowerPlantID = id
            };
            PowerPlantDataModel basicsOfPowerPlant = await GetBasicsOfPowerPlant(id);

            PowerPlant.name = basicsOfPowerPlant.name;
            PowerPlant.description = basicsOfPowerPlant.description;
            PowerPlant.OperatorCompany = basicsOfPowerPlant.OperatorCompany;
            PowerPlant.webpage = basicsOfPowerPlant.webpage;
            PowerPlant.Color = basicsOfPowerPlant.color;
            PowerPlant.Address = basicsOfPowerPlant.address;
            PowerPlant.IsCountry = basicsOfPowerPlant.IsCountry;
            PowerPlant.longitude = Math.Round(basicsOfPowerPlant.longitude, 4);
            PowerPlant.latitude = Math.Round(basicsOfPowerPlant.latitude, 4);

            List<PowerPlantDetailsModel> powerPlantDetails = await _powerRepository.QueryPowerPlantDetails(id);

            List<DateTime> timeStampsUtc = await _dateService.HandleWhichDateFormatIsBeingUsed(date, startLocal, endLocal);
            PowerPlant.DataStart = timeStampsUtc[0];
            PowerPlant.DataEnd = timeStampsUtc[1];

            if (date != null)
            {
                string msg = await CheckWhetherDataIsPresentInTheGivenTimePeriod(timeStampsUtc);
                System.Diagnostics.Debug.WriteLine(msg);
            }

            int PPMaxPower = 0, PPCurrentPower = 0, i = 0;
            List<BlocDTO> blocs = new();
            while (i < powerPlantDetails.Count)
            {
                BlocDTO bloc = new()
                {
                    BlocID = powerPlantDetails[i].BlocId,
                    BlocType = powerPlantDetails[i].BlocType,
                    MaxBlocCapacity = powerPlantDetails[i].MaxBlocCapacity,
                    ComissionDate = powerPlantDetails[i].ComissionDate
                };

                List<GeneratorDTO> generators = new();
                int CurrentPower = 0, MaxPower = 0;
                while (i < powerPlantDetails.Count && powerPlantDetails[i].BlocId == bloc.BlocID)
                {
                    GeneratorDTO generator = new()
                    {
                        GeneratorID = powerPlantDetails[i].GeneratorID,
                        MaxCapacity = powerPlantDetails[i].MaxCapacity
                    };
                    generator.PastPower = await _powerHelper.GetGeneratorPower(generator.GeneratorID, timeStampsUtc[0], timeStampsUtc[1]);
                    generators.Add(generator);
                    CurrentPower += generator.PastPower[generator.PastPower.Count - 1].Power;
                    MaxPower += generator.MaxCapacity;
                    i++;
                }
                bloc.CurrentPower = CurrentPower;
                bloc.MaxPower = MaxPower;
                bloc.Generators = generators;
                blocs.Add(bloc);

                PPCurrentPower += CurrentPower;
                PPMaxPower += MaxPower;
            }

            PowerPlant.Blocs = blocs;
            PowerPlant.CurrentPower = PPCurrentPower;
            PowerPlant.MaxPower = PPMaxPower;

            return PowerPlant;
        }

        public async Task<IEnumerable<PowerStampDTO>> GetPowerOfPowerPlant(string id, DateTime? date = null, DateTime? startLocal = null, DateTime? endLocal = null)
        {
            List<DateTime> timeStampsUtc = await _dateService.HandleWhichDateFormatIsBeingUsed(date, startLocal, endLocal);
            int numberOfDataPoints = _dateService.CalculateTheNumberOfIntervals(timeStampsUtc[0], timeStampsUtc[1]);
            List<PowerStampDTO> powerStamps = await _powerHelper.GetPowerStampsListOfPowerPlant(id, numberOfDataPoints, timeStampsUtc);
            return powerStamps;
        }

        public async Task<PowerOfPowerPlantsDTO> GetPowerOfPowerPlants(DateTime? date = null, DateTime? startLocal = null, DateTime? endLocal = null)
        {
            PowerOfPowerPlantsDTO powerOfPowerPlants = new PowerOfPowerPlantsDTO();
            List<string> powerPlants = await _powerRepository.QueryPowerPlants();

            List<DateTime> timeStampsUtc = await _dateService.HandleWhichDateFormatIsBeingUsed(date, startLocal, endLocal);
            powerOfPowerPlants.Start = timeStampsUtc[0]; //Utc
            powerOfPowerPlants.End = timeStampsUtc[1]; //Utc

            if (date != null)
            {
                string msg = await CheckWhetherDataIsPresentInTheGivenTimePeriod(timeStampsUtc);
                System.Diagnostics.Debug.WriteLine(msg);
            }

            int numberOfDataPoints = _dateService.CalculateTheNumberOfIntervals(powerOfPowerPlants.Start, powerOfPowerPlants.End);

            List<PowerOfPowerPlantDTO> data = new();

            foreach (string powerPlant in powerPlants)
            {
                PowerOfPowerPlantDTO powerOfPowerPlant = new();
                powerOfPowerPlant.PowerPlantName = powerPlant;
                powerOfPowerPlant.PowerStamps = await _powerHelper.GetPowerStampsListOfPowerPlant(powerPlant, numberOfDataPoints, timeStampsUtc);
                data.Add(powerOfPowerPlant);
            }

            powerOfPowerPlants.Data = data;
            return powerOfPowerPlants;
        }

        public async Task<string> InitData(DateTime? periodStart = null, DateTime? periodEnd = null)
        {
            List<DateTime> timeStampsUtc = new();
            if (periodStart is DateTime time && periodEnd is DateTime time1)
            {
                periodStart = DateTime.SpecifyKind(time, DateTimeKind.Local);
                periodEnd = DateTime.SpecifyKind(time1, DateTimeKind.Local);
                timeStampsUtc.Add(TimeZoneInfo.ConvertTimeToUtc(periodStart.Value, TimeZoneInfo.Local));
                timeStampsUtc.Add(TimeZoneInfo.ConvertTimeToUtc(periodEnd.Value, TimeZoneInfo.Local));
            }
            else
            {
                timeStampsUtc = await _dateService.GetInitDataTimeInterval();
            }

            if ((timeStampsUtc[1] - timeStampsUtc[0]).TotalHours <= 24)
            {
                var task1 = Task.Run(async () => await GetPPData("A73", timeStampsUtc));
                var task2 = Task.Run(async () => await GetPPData("A75", timeStampsUtc));
                var task3 = Task.Run(async () => await GetImportData("10YHU-MAVIR----U", timeStampsUtc));

                await Task.WhenAll(task1, task2, task3);
            }
            else
            {
                DateTime currentTime = timeStampsUtc[0];
                while (currentTime < timeStampsUtc[1])
                {
                    DateTime End = currentTime.AddHours(24);
                    if (End > timeStampsUtc[1])
                    {
                        End = timeStampsUtc[1];
                    }

                    await InitData(currentTime, End);
                    currentTime = currentTime.AddHours(24);
                }
            }

            List<DateTime> LastData = await _powerRepository.QueryLastDataTime();
            return timeStampsUtc[0] + " - " + timeStampsUtc[1] + " --> " + LastData[0];
        }

        private async Task GetPPData(string docType, List<DateTime> timeStampsUtc)
        {
            if (docType == "A73" || docType == "A75")
            {
                try
                {
                    List<string> Generators = await _powerRepository.QueryGenerators();

                    XDocument document = XDocument.Parse(await _powerHelper.APIquery(docType, timeStampsUtc[0], timeStampsUtc[1]));
                    XNamespace ns = document?.Root.Name.Namespace;

                    if (document is not null && document?.Root is not null && document?.Root?.Elements(ns + "TimeSeries") is not null)
                    {
                        foreach (var TimeSeries in document?.Root?.Elements(ns + "TimeSeries"))
                        {
                            DateTime startTimePointUtc = timeStampsUtc[0];
                            string? generatorName = "";
                            if (docType == "A73")
                            {
                                generatorName = TimeSeries?.Element(ns + "MktPSRType")?.Element(ns + "PowerSystemResources")?.Element(ns + "name")?.Value;
                            }
                            if (docType == "A75")
                            {
                                generatorName = TimeSeries?.Element(ns + "MktPSRType")?.Element(ns + "psrType")?.Value;
                            }

                            if (generatorName is not null && Generators.Contains(generatorName))
                            {
                                XElement Period = TimeSeries.Element(ns + "Period");
                                List<int> power = new List<int>();
                                if (Period is not null)
                                {
                                    foreach (XElement Point in Period.Elements(ns + "Point"))
                                    {
                                        int currentPower = Convert.ToInt32(Point?.Element(ns + "quantity")?.Value);
                                        await _powerRepository.InsertData(generatorName, startTimePointUtc, currentPower);
                                        startTimePointUtc = startTimePointUtc.AddMinutes(15);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception Exception)
                {
                    Console.WriteLine(Exception);
                }
            }
            else
            {
                throw new NotImplementedException("Unimplemented DocumentType was given");
            }
        }

        private async Task GetImportData(string homeCountry, List<DateTime> timeStampsUtc)
        {
            List<string> neighbourCountries = new()
            {
                "10YSK-SEPS-----K",
                "10YAT-APG------L",
                "10YSI-ELES-----O",
                "10YHR-HEP------M",
                "10YCS-SERBIATSOV",
                "10YRO-TEL------P",
                "10Y1001C--00003F"
            };

            List<string> problematicCountries = new()
            {
                "10YSK-SEPS-----K",
                "10YHR-HEP------M",
                "10YCS-SERBIATSOV",
                "10Y1001C--00003F"
            };

            foreach (string countryCode in neighbourCountries)
            {
                DateTime startTimePointUtc = timeStampsUtc[0];
                DateTime queryStartTimeUtc = timeStampsUtc[0];
                DateTime queryEndTimeUtc = timeStampsUtc[1];
                
                if (problematicCountries.Contains(countryCode))
                {
                    queryStartTimeUtc = queryStartTimeUtc.AddMinutes(queryStartTimeUtc.Minute * -1);
                    queryEndTimeUtc = queryEndTimeUtc.AddMinutes(queryEndTimeUtc.Minute * -1);
                }

                try
                {
                    XDocument importedEnergyData = XDocument.Parse(await _powerHelper.APIquery("A11", queryStartTimeUtc, queryEndTimeUtc, homeCountry, countryCode));
                    XDocument exportedEnergyData = XDocument.Parse(await _powerHelper.APIquery("A11", queryStartTimeUtc, queryEndTimeUtc, countryCode, homeCountry));

                    XNamespace importNameSpace = importedEnergyData.Root.Name.Namespace;
                    XNamespace exportNameSpace = exportedEnergyData.Root.Name.Namespace;

                    var importTimeSeries = importedEnergyData?.Root?.Element(importNameSpace + "TimeSeries");
                    var exportTimeSeries = exportedEnergyData?.Root?.Element(exportNameSpace + "TimeSeries");

                    if (importTimeSeries?.Elements() is not null && exportTimeSeries?.Elements() is not null)
                    {
                        var importPeriod = importTimeSeries?.Element(importNameSpace + "Period");
                        var exportPeriod = exportTimeSeries?.Element(exportNameSpace + "Period");

                        List<PowerStampDTO> powerStamps = new List<PowerStampDTO>();

                        if (importPeriod?.Elements(importNameSpace + "Point") is not null && exportPeriod?.Elements(exportNameSpace + "Point") is not null)
                        {
                            for (int i = 0; i < importPeriod.Elements(importNameSpace + "Point").Count(); i++)
                            {
                                XElement importPoint = importPeriod.Elements(importNameSpace + "Point").ToList()[i];
                                XElement exportPoint = exportPeriod.Elements(importNameSpace + "Point").ToList()[i];

                                int importValue = Convert.ToInt32(importPoint?.Element(importNameSpace + "quantity")?.Value);
                                int exportValue = Convert.ToInt32(exportPoint?.Element(exportNameSpace + "quantity")?.Value);
                                exportValue *= -1;

                                int currentPower = importValue + exportValue;

                                int numberOfTimesTheValueHasToBeSaved = 1;
                                if (problematicCountries.Contains(countryCode))
                                {
                                    numberOfTimesTheValueHasToBeSaved = 4;
                                }

                                for (int j = 0; j < numberOfTimesTheValueHasToBeSaved; j++)
                                {
                                    await _powerRepository.InsertData(countryCode, startTimePointUtc, currentPower);
                                    startTimePointUtc = startTimePointUtc.AddMinutes(15);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
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
