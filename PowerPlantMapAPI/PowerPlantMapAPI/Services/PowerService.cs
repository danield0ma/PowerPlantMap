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
                var task1 = Task.Run(async () => await GetPPData("A73", TimeStamps));
                var task2 = Task.Run(async () => await GetPPData("A75", TimeStamps));
                var task3 = Task.Run(async () => await GetImportData("10YHU-MAVIR----U", TimeStamps));

                Task.WaitAll(task1, task2, task3);
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

        private async Task GetPPData(string docType, List<DateTime> TimeStamps)
        {
            string periodStart = _dateService.EditTime(TimeStamps[0]);
            string periodEnd = _dateService.EditTime(TimeStamps[1]);
            DateTime startTimePoint = TimeStamps[0];

            if (docType == "A73" || docType == "A75")
            {
                try
                {
                    List<string> Generators = await _powerRepository.QueryGenerators();

                    XDocument document = XDocument.Parse(await _powerHelper.APIquery(docType, periodStart, periodEnd));
                    XNamespace ns = document?.Root.Name.Namespace;

                    if (document is not null && document?.Root is not null && document?.Root?.Elements(ns + "TimeSeries") is not null)
                    {
                        foreach (var TimeSeries in document?.Root?.Elements(ns + "TimeSeries"))
                        {
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
                                        power.Add(currentPower);
                                        await _powerRepository.InsertData(generatorName, startTimePoint, currentPower);
                                        startTimePoint = startTimePoint.AddMinutes(15);
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

        private async Task GetImportData(string homeCountry, List<DateTime> TimeStamps)
        {
            string periodStart = _dateService.EditTime(TimeStamps[0]);
            string periodEnd = _dateService.EditTime(TimeStamps[1]);
            DateTime startTimePoint = TimeStamps[0];

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
                if (problematicCountries.Contains(countryCode))
                {
                    periodStart = periodStart.Remove(10);
                    periodStart += "00";
                    periodEnd = periodEnd.Remove(10);
                    periodEnd += "00";
                }

                try
                {
                    XDocument importedEnergyData = XDocument.Parse(await _powerHelper.APIquery("A11", periodStart, periodEnd, homeCountry, countryCode));
                    XDocument exportedEnergyData = XDocument.Parse(await _powerHelper.APIquery("A11", periodStart, periodEnd, countryCode, homeCountry));

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
                                    await _powerRepository.InsertData(countryCode, startTimePoint, currentPower);
                                    startTimePoint = startTimePoint.AddMinutes(15);
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
