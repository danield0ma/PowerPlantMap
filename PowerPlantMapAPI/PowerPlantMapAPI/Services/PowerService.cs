using Microsoft.AspNetCore.Mvc;
using PowerPlantMapAPI.Models;
using PowerPlantMapAPI.Models.DTO;
using PowerPlantMapAPI.Repositories;
using PowerPlantMapAPI.Helpers;
using System.Xml.Linq;

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

        public async Task<ActionResult<IEnumerable<PowerPlantBasicsModel>>> GetPowerPlantBasics()
        {
            var powerPlantBasics = new List<PowerPlantBasicsModel>();
            var dataOfPowerPlants = await _powerRepository.GetDataOfPowerPlants();

            foreach (var dataOfPowerPlant in dataOfPowerPlants)
            {
                var feature = new PowerPlantBasicsModel();
                var properties = new FeaturePropertyDto
                {
                    Id = dataOfPowerPlant.PowerPlantId,
                    Name = dataOfPowerPlant.Name,
                    Description = dataOfPowerPlant.Description,
                    Img = dataOfPowerPlant.Image
                };
                feature.Properties = properties;

                var coordinates = new List<float>
                {
                    dataOfPowerPlant.Latitude,
                    dataOfPowerPlant.Longitude
                };
                var geometry = new FeatureGeometryDto
                {
                    Type = "Point",
                    Coordinates = coordinates
                };
                feature.Geometry = geometry;
                powerPlantBasics.Add(feature);
            }

            return powerPlantBasics;
        }
        
        public async Task<ActionResult<PowerPlantDetailsModel>> GetDetailsOfPowerPlant(string id, DateTime? date = null, DateTime? startLocal = null, DateTime? endLocal = null)
        {
            var basicsOfPowerPlant = await _powerRepository.GetDataOfPowerPlant(id);
            
            PowerPlantDetailsModel detailsOfPowerPlant = new()
            {
                PowerPlantId = id,
                Name = basicsOfPowerPlant.Name,
                Description = basicsOfPowerPlant.Description,
                OperatorCompany = basicsOfPowerPlant.OperatorCompany,
                Webpage = basicsOfPowerPlant.Webpage,
                Color = basicsOfPowerPlant.Color,
                Address = basicsOfPowerPlant.Address,
                IsCountry = basicsOfPowerPlant.IsCountry,
                Longitude = Math.Round(basicsOfPowerPlant.Longitude, 4),
                Latitude = Math.Round(basicsOfPowerPlant.Latitude, 4)
            };
            
            var timeStampsUtc = await _dateService.HandleWhichDateFormatIsBeingUsed(date, startLocal, endLocal);
            detailsOfPowerPlant.DataStart = timeStampsUtc[0];
            detailsOfPowerPlant.DataEnd = timeStampsUtc[1];
            
            if (date != null)
            {
                var msg = await CheckWhetherDataIsPresentInTheGivenTimePeriod(timeStampsUtc);
                System.Diagnostics.Debug.WriteLine(msg);
            }

            int maxPowerOfPowerPlant = 0, currentPowerOfPowerPlant = 0, i = 0;
            List<BlocDto> blocsOfPowerPlant = new();
            
            var powerPlantDetails = await _powerRepository.GetPowerPlantDetails(id);
            
            while (i < powerPlantDetails.Count)
            {
                BlocDto bloc = new()
                {
                    BlocId = powerPlantDetails[i].BlocId,
                    BlocType = powerPlantDetails[i].BlocType,
                    MaxBlocCapacity = powerPlantDetails[i].MaxBlocCapacity,
                    CommissionDate = powerPlantDetails[i].CommissionDate
                };

                List<GeneratorDto> generators = new();
                int currentPower = 0, maxPower = 0;
                while (i < powerPlantDetails.Count && powerPlantDetails[i].BlocId == bloc.BlocId)
                {
                    GeneratorDto generator = new()
                    {
                        GeneratorId = powerPlantDetails[i].GeneratorId,
                        MaxCapacity = powerPlantDetails[i].MaxCapacity
                    };
                    generator.PastPower = await _powerHelper.GetGeneratorPower(generator.GeneratorId, timeStampsUtc[0], timeStampsUtc[1]);
                    generators.Add(generator);
                    currentPower += generator.PastPower![generator.PastPower.Count - 1].Power;
                    maxPower += generator.MaxCapacity;
                    i++;
                }
                bloc.CurrentPower = currentPower;
                bloc.MaxPower = maxPower;
                bloc.Generators = generators;
                blocsOfPowerPlant.Add(bloc);

                currentPowerOfPowerPlant += currentPower;
                maxPowerOfPowerPlant += maxPower;
            }

            detailsOfPowerPlant.Blocs = blocsOfPowerPlant;
            detailsOfPowerPlant.CurrentPower = currentPowerOfPowerPlant;
            detailsOfPowerPlant.MaxPower = maxPowerOfPowerPlant;

            return detailsOfPowerPlant;
        }

        public async Task<IEnumerable<PowerOfPowerPlantModel>> GetPowerOfPowerPlant(string id, DateTime? date = null, DateTime? start = null, DateTime? end = null)
        {
            var timeStampsUtc = await _dateService.HandleWhichDateFormatIsBeingUsed(date, start, end);
            var numberOfDataPoints = _dateService.CalculateTheNumberOfIntervals(timeStampsUtc[0], timeStampsUtc[1]);
            var powerStamps = await _powerHelper.GetPowerStampsListOfPowerPlant(id, numberOfDataPoints, timeStampsUtc);
            return powerStamps;
        }

        public async Task<PowerOfPowerPlantsModel> GetPowerOfPowerPlants(DateTime? date = null, DateTime? start = null, DateTime? end = null)
        {
            var powerOfPowerPlants = new PowerOfPowerPlantsModel();
            var powerPlants = await _powerRepository.GetPowerPlantNames();

            var timeStampsUtc = await _dateService.HandleWhichDateFormatIsBeingUsed(date, start, end);
            powerOfPowerPlants.Start = timeStampsUtc[0]; //Utc
            powerOfPowerPlants.End = timeStampsUtc[1]; //Utc

            if (date != null)
            {
                var msg = await CheckWhetherDataIsPresentInTheGivenTimePeriod(timeStampsUtc);
                System.Diagnostics.Debug.WriteLine(msg);
            }

            var numberOfDataPoints = _dateService.CalculateTheNumberOfIntervals(powerOfPowerPlants.Start, powerOfPowerPlants.End);

            List<PowerOfPowerPlantDto> data = new();

            foreach (var powerPlant in powerPlants)
            {
                PowerOfPowerPlantDto powerOfPowerPlant = new()
                {
                    PowerPlantName = powerPlant,
                    PowerStamps = await _powerHelper.GetPowerStampsListOfPowerPlant(powerPlant, numberOfDataPoints, timeStampsUtc)
                };
                data.Add(powerOfPowerPlant);
            }

            powerOfPowerPlants.Data = data;
            return powerOfPowerPlants;
        }

        public async Task<string> InitData(DateTime? periodStart = null, DateTime? periodEnd = null)
        {
            List<DateTime> timeStampsUtc = new();
            if (periodStart is { } time && periodEnd is { } time1)
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
                var task1 = Task.Run(async () => await GetPowerPlantData("A73", timeStampsUtc));
                var task2 = Task.Run(async () => await GetPowerPlantData("A75", timeStampsUtc));
                var task3 = Task.Run(async () => await GetImportAndExportData("10YHU-MAVIR----U", timeStampsUtc));

                await Task.WhenAll(task1, task2, task3);
            }
            else
            {
                var currentTime = timeStampsUtc[0];
                while (currentTime < timeStampsUtc[1])
                {
                    var end = currentTime.AddHours(24);
                    if (end > timeStampsUtc[1])
                    {
                        end = timeStampsUtc[1];
                    }

                    await InitData(currentTime, end);
                    currentTime = currentTime.AddHours(24);
                }
            }

            var lastData = await _powerRepository.GetLastDataTime();
            return timeStampsUtc[0] + " - " + timeStampsUtc[1] + " --> " + lastData[0];
        }

        private async Task GetPowerPlantData(string docType, IReadOnlyList<DateTime> timeStampsUtc)
        {
            if (docType is "A73" or "A75")
            {
                try
                {
                    var generators = await _powerRepository.GetGeneratorNames();

                    var document = XDocument.Parse(await _powerHelper.ApiQuery(docType, timeStampsUtc[0], timeStampsUtc[1]));
                    var ns = document.Root?.Name.Namespace;

                    if (document.Root is not null && ns is not null && document.Root?.Elements(ns + "TimeSeries") is not null)
                    {
                        foreach (var timeSeries in document.Root?.Elements(ns + "TimeSeries")!)
                        {
                            var startTimePointUtc = timeStampsUtc[0];
                            var generatorName = docType switch
                            {
                                "A73" => timeSeries.Element(ns + "MktPSRType")
                                    ?.Element(ns + "PowerSystemResources")
                                    ?.Element(ns + "name")
                                    ?.Value,
                                "A75" => timeSeries.Element(ns + "MktPSRType")?.Element(ns + "psrType")?.Value,
                                _ => ""
                            };

                            if (generatorName is null || !generators.Contains(generatorName)) continue;
                            var period = timeSeries.Element(ns + "Period");
                            if (period is null) continue;
                            foreach (var point in period.Elements(ns + "Point"))
                            {
                                var currentPower = Convert.ToInt32(point.Element(ns + "quantity")?.Value);
                                await _powerRepository.AddPastActivity(generatorName, startTimePointUtc, currentPower);
                                startTimePointUtc = startTimePointUtc.AddMinutes(15);
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
            else
            {
                throw new NotImplementedException("Unimplemented DocumentType was given");
            }
        }

        private async Task GetImportAndExportData(string homeCountry, IReadOnlyList<DateTime> timeStampsUtc)
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

            foreach (var countryCode in neighbourCountries)
            {
                var startTimePointUtc = timeStampsUtc[0];
                var getStartTimeUtc = timeStampsUtc[0];
                var getEndTimeUtc = timeStampsUtc[1];
                
                if (problematicCountries.Contains(countryCode))
                {
                    getStartTimeUtc = getStartTimeUtc.AddMinutes(getStartTimeUtc.Minute * -1);
                    getEndTimeUtc = getEndTimeUtc.AddMinutes(getEndTimeUtc.Minute * -1);
                }

                try
                {
                    var importedEnergyData = XDocument.Parse(await _powerHelper.ApiQuery("A11", getStartTimeUtc, getEndTimeUtc, homeCountry, countryCode));
                    var exportedEnergyData = XDocument.Parse(await _powerHelper.ApiQuery("A11", getStartTimeUtc, getEndTimeUtc, countryCode, homeCountry));

                    var importNameSpace = importedEnergyData.Root?.Name.Namespace;
                    var exportNameSpace = exportedEnergyData.Root?.Name.Namespace;

                    if (importNameSpace is not null && exportNameSpace is not null)
                    {
                       var importTimeSeries = importedEnergyData.Root?.Element(importNameSpace + "TimeSeries");
                       var exportTimeSeries = exportedEnergyData.Root?.Element(exportNameSpace + "TimeSeries");
   
                       if (importTimeSeries?.Elements() is not null && exportTimeSeries?.Elements() is not null)
                       {
                           var importPeriod = importTimeSeries.Element(importNameSpace + "Period");
                           var exportPeriod = exportTimeSeries.Element(exportNameSpace + "Period");
   
                           if (importPeriod?.Elements(importNameSpace + "Point") is not null && exportPeriod?.Elements(exportNameSpace + "Point") is not null)
                           {
                               for (var i = 0; i < importPeriod.Elements(importNameSpace + "Point").Count(); i++)
                               {
                                   var importPoint = importPeriod.Elements(importNameSpace + "Point").ToList()[i];
                                   var exportPoint = exportPeriod.Elements(importNameSpace + "Point").ToList()[i];
   
                                   var importValue = Convert.ToInt32(importPoint.Element(importNameSpace + "quantity")?.Value);
                                   var exportValue = Convert.ToInt32(exportPoint.Element(exportNameSpace + "quantity")?.Value);
                                   exportValue *= -1;
   
                                   var currentPower = importValue + exportValue;
   
                                   var numberOfTimesTheValueHasToBeSaved = 1;
                                   if (problematicCountries.Contains(countryCode))
                                   {
                                       numberOfTimesTheValueHasToBeSaved = 4;
                                   }
   
                                   for (var j = 0; j < numberOfTimesTheValueHasToBeSaved; j++)
                                   {
                                       await _powerRepository.AddPastActivity(countryCode, startTimePointUtc, currentPower);
                                       startTimePointUtc = startTimePointUtc.AddMinutes(15);
                                   }
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

        private async Task<string> CheckWhetherDataIsPresentInTheGivenTimePeriod(IReadOnlyList<DateTime> timeStamps)
        {
            var pastActivity = await _powerRepository.GetPastActivity("PA_gép1", timeStamps[0], timeStamps[1]);

            if (pastActivity.Count < 10)
            {
                return await InitData(timeStamps[0].AddHours(-2), timeStamps[1].AddHours(2));
            }
            return "no InitData";
        }
    }
}
