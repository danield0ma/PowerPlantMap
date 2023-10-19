using Microsoft.AspNetCore.Mvc;
using PowerPlantMapAPI.Models;
using PowerPlantMapAPI.Models.DTO;
using PowerPlantMapAPI.Repositories;
using PowerPlantMapAPI.Helpers;

namespace PowerPlantMapAPI.Services
{
    public class PowerService : IPowerService
    {
        private readonly IPowerRepository _powerRepository;
        private readonly IDateHelper _dateHelper;
        private readonly IPowerHelper _powerHelper;
        private readonly IXmlHelper _xmlHelper;

        public PowerService(
            IDateHelper dateHelper, 
            IPowerRepository repository,
            IPowerHelper powerHelper,
            IXmlHelper xmlHelper)
        {
            _dateHelper = dateHelper;
            _powerRepository = repository;
            _powerHelper = powerHelper;
            _xmlHelper = xmlHelper;
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
            var powerPlantData = await _powerRepository.GetDataOfPowerPlant(id);
            
            PowerPlantDetailsModel detailsOfPowerPlant = new()
            {
                PowerPlantId = id,
                Name = powerPlantData.Name,
                Description = powerPlantData.Description,
                OperatorCompany = powerPlantData.OperatorCompany,
                Webpage = powerPlantData.Webpage,
                Color = powerPlantData.Color,
                Address = powerPlantData.Address,
                IsCountry = powerPlantData.IsCountry,
                Longitude = Math.Round(powerPlantData.Longitude, 4),
                Latitude = Math.Round(powerPlantData.Latitude, 4)
            };
            
            var timeStampsUtc = await _dateHelper.HandleWhichDateFormatIsBeingUsed(date, startLocal, endLocal);
            detailsOfPowerPlant.DataStart = timeStampsUtc[0];
            detailsOfPowerPlant.DataEnd = timeStampsUtc[1];
            
            if (date != null || startLocal != null && endLocal != null)
            {
                var msg = await CheckWhetherDataIsPresentInTheGivenTimePeriod(timeStampsUtc);
                System.Diagnostics.Debug.WriteLine(msg);
            }

            int maxPowerOfPowerPlant = 0, currentPowerOfPowerPlant = 0;
            List<BlocDto> blocsOfPowerPlant = new();
            
            var powerPlantDetails = await _powerRepository.GetPowerPlantDetails(id);

            foreach (var powerPlantDetail in powerPlantDetails.GroupBy(x => x.BlocId ).Select(group => group.First()).ToList())
            {
                BlocDto bloc = new()
                {
                    BlocId = powerPlantDetail.BlocId,
                    BlocType = powerPlantDetail.BlocType,
                    MaxBlocCapacity = powerPlantDetail.MaxBlocCapacity,
                    CommissionDate = powerPlantDetail.CommissionDate
                };

                List<GeneratorDto> generators = new();
                int currentPower = 0, maxPower = 0;

                foreach (var item in powerPlantDetails.Where(x => x.BlocId == powerPlantDetail.BlocId).ToList())
                {
                    GeneratorDto generator = new()
                    {
                        GeneratorId = item.GeneratorId,
                        MaxCapacity = item.MaxCapacity,
                        PastPower = await _powerHelper.GetGeneratorPower(item.GeneratorId, timeStampsUtc[0], timeStampsUtc[1])
                    };
                    
                    generators.Add(generator);
                    currentPower += generator.PastPower![generator.PastPower.Count - 1].Power;
                    maxPower += generator.MaxCapacity;
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
            var timeStampsUtc = await _dateHelper.HandleWhichDateFormatIsBeingUsed(date, start, end);
            var numberOfDataPoints = _dateHelper.CalculateTheNumberOfIntervals(timeStampsUtc[0], timeStampsUtc[1]);
            var powerStamps = await _powerHelper.GetPowerStampsListOfPowerPlant(id, numberOfDataPoints, timeStampsUtc);
            return powerStamps;
        }

        public async Task<PowerOfPowerPlantsModel> GetPowerOfPowerPlants(DateTime? date = null, DateTime? start = null, DateTime? end = null)
        {
            var powerOfPowerPlants = new PowerOfPowerPlantsModel();
            var powerPlants = await _powerRepository.GetPowerPlantNames();

            var timeStampsUtc = await _dateHelper.HandleWhichDateFormatIsBeingUsed(date, start, end);
            powerOfPowerPlants.Start = timeStampsUtc[0]; //Utc
            powerOfPowerPlants.End = timeStampsUtc[1]; //Utc

            if (date != null)
            {
                var msg = await CheckWhetherDataIsPresentInTheGivenTimePeriod(timeStampsUtc);
                System.Diagnostics.Debug.WriteLine(msg);
            }

            var numberOfDataPoints = _dateHelper.CalculateTheNumberOfIntervals(powerOfPowerPlants.Start, powerOfPowerPlants.End);

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
                timeStampsUtc = await _dateHelper.GetInitDataTimeInterval();
            }

            if ((timeStampsUtc[1] - timeStampsUtc[0]).TotalHours <= 24)
            {
                var task1 = Task.Run(async () => await _xmlHelper.GetPowerPlantData("A73", timeStampsUtc));
                var task2 = Task.Run(async () => await _xmlHelper.GetPowerPlantData("A75", timeStampsUtc));
                var task3 = Task.Run(async () => await _xmlHelper.GetImportAndExportData("10YHU-MAVIR----U", timeStampsUtc));

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
