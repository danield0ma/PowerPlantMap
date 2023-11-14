using Microsoft.AspNetCore.Mvc;
using PowerPlantMapAPI.Data;
using PowerPlantMapAPI.Data.Dto;
using PowerPlantMapAPI.Repositories;
using PowerPlantMapAPI.Helpers;

namespace PowerPlantMapAPI.Services;

public class PowerDataService : IPowerDataService
{
    private readonly IPowerPlantRepository _powerPlantRepository;
    private readonly IPowerDataRepository _powerDataRepository;
    private readonly IDateHelper _dateHelper;
    private readonly IPowerHelper _powerHelper;
    private readonly IXmlHelper _xmlHelper;
    private readonly ILogger<PowerDataService> _logger;

    public PowerDataService(
        IDateHelper dateHelper, 
        IPowerPlantRepository powerPlantRepository,
        IPowerDataRepository powerDataRepository,
        IPowerHelper powerHelper,
        IXmlHelper xmlHelper,
        ILogger<PowerDataService> logger)
    {
        _dateHelper = dateHelper;
        _powerPlantRepository = powerPlantRepository;
        _powerDataRepository = powerDataRepository;
        _powerHelper = powerHelper;
        _xmlHelper = xmlHelper;
        _logger = logger;
    }

    public async Task<ActionResult<IEnumerable<PowerPlantBasicsModel>>> GetPowerPlantBasics()
    {
        var powerPlantBasics = new List<PowerPlantBasicsModel>();
        var dataOfPowerPlants = await _powerDataRepository.GetDataOfPowerPlants();

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
    
    public async Task<ActionResult<PowerPlantDetailsModel>> GetDetailsOfPowerPlant(
        string id, DateTime? date = null, DateTime? startLocal = null, DateTime? endLocal = null)
    {
        var powerPlantData = await _powerDataRepository.GetDataOfPowerPlant(id);
        
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
            await CheckWhetherDataIsPresentInTheGivenTimePeriod(timeStampsUtc);
        }

        int maxPowerOfPowerPlant = 0, currentPowerOfPowerPlant = 0;
        List<BlocDetailsDto> blocsOfPowerPlant = new();
        
        var powerPlantDetails = await _powerPlantRepository.GetPowerPlantDetails(id);

        foreach (var powerPlantDetail in powerPlantDetails.
                     GroupBy(x => x.BlocId ).
                     Select(group => group.First()).ToList())
        {
            BlocDetailsDto blocDetails = new()
            {
                BlocId = powerPlantDetail.BlocId,
                BlocType = powerPlantDetail.BlocType,
                MaxBlocCapacity = powerPlantDetail.MaxBlocCapacity,
                CommissionDate = powerPlantDetail.CommissionDate
            };

            List<GeneratorDetailsDto> generators = new();
            int currentPower = 0, maxPower = 0;

            foreach (var item in powerPlantDetails.
                         Where(x => x.BlocId == powerPlantDetail.BlocId).ToList())
            {
                GeneratorDetailsDto generatorDetails = new()
                {
                    GeneratorId = item.GeneratorId,
                    MaxCapacity = item.MaxCapacity,
                    PastPower = await _powerHelper.
                        GetGeneratorPower(item.GeneratorId, timeStampsUtc[0], timeStampsUtc[1])
                };
                
                generators.Add(generatorDetails);
                currentPower += generatorDetails.PastPower![generatorDetails.PastPower.Count - 1].Power;
                maxPower += generatorDetails.MaxCapacity;
            }
            
            blocDetails.CurrentPower = currentPower;
            blocDetails.MaxPower = maxPower;
            blocDetails.Generators = generators;
            blocsOfPowerPlant.Add(blocDetails);

            currentPowerOfPowerPlant += currentPower;
            maxPowerOfPowerPlant += maxPower;
        }
        
        detailsOfPowerPlant.Blocs = blocsOfPowerPlant;
        detailsOfPowerPlant.CurrentPower = currentPowerOfPowerPlant;
        detailsOfPowerPlant.MaxPower = maxPowerOfPowerPlant;

        return detailsOfPowerPlant;
    }

    public async Task<IEnumerable<PowerOfPowerPlantModel>> GetPowerOfPowerPlant(
        string id, DateTime? date = null, DateTime? start = null, DateTime? end = null)
    {
        var timeStampsUtc = await _dateHelper.HandleWhichDateFormatIsBeingUsed(date, start, end);
        var numberOfDataPoints = _dateHelper.CalculateTheNumberOfIntervals(timeStampsUtc[0], timeStampsUtc[1]);
        var powerStamps = await _powerHelper.
            GetPowerStampsListOfPowerPlant(id, numberOfDataPoints, timeStampsUtc);
        return powerStamps;
    }

    public async Task<PowerOfPowerPlantsModel> GetPowerOfPowerPlants(
        DateTime? date = null, DateTime? start = null, DateTime? end = null)
    {
        var powerOfPowerPlants = new PowerOfPowerPlantsModel();
        var powerPlants = await _powerPlantRepository.GetPowerPlantNames();

        var timeStampsUtc = await _dateHelper.HandleWhichDateFormatIsBeingUsed(date, start, end);
        powerOfPowerPlants.Start = timeStampsUtc[0]; //Utc
        powerOfPowerPlants.End = timeStampsUtc[1]; //Utc

        if (date != null)
        {
            await CheckWhetherDataIsPresentInTheGivenTimePeriod(timeStampsUtc);
        }

        var numberOfDataPoints = _dateHelper.CalculateTheNumberOfIntervals(
            powerOfPowerPlants.Start, powerOfPowerPlants.End);

        List<PowerOfPowerPlantDto> data = new();

        foreach (var powerPlant in powerPlants)
        {
            PowerOfPowerPlantDto powerOfPowerPlant = new()
            {
                PowerPlantName = powerPlant,
                PowerStamps = await _powerHelper.GetPowerStampsListOfPowerPlant(
                    powerPlant, numberOfDataPoints, timeStampsUtc)
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
            var task3 = Task.Run(async () => await _xmlHelper.GetImportAndExportData(
                "10YHU-MAVIR----U", timeStampsUtc));

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

        var lastData = await _powerDataRepository.GetLastDataTime();
        return timeStampsUtc[0] + " - " + timeStampsUtc[1] + " --> " + lastData[0];
    }

    private async Task CheckWhetherDataIsPresentInTheGivenTimePeriod(IReadOnlyList<DateTime> timeStamps)
    {
        var count = 0;
        var difference = timeStamps[1].Subtract(timeStamps[0]);
        var totalMinutes = difference.TotalMinutes;
        var generatorNames = await _powerDataRepository.GetGeneratorNames();
        foreach (var generatorName in generatorNames)
        {
            var pastActivity = await _powerDataRepository.GetPastActivity(
                        generatorName, timeStamps[0], timeStamps[1]);
            count += pastActivity.Count;
        }

        _logger.LogInformation("count: {Count}, difference: {Difference}, /15: {TotalMinutes}", count, difference, totalMinutes);
        if (!(count < generatorNames.Count * (totalMinutes / 15) * 0.9)) return;
        await InitData(timeStamps[0].AddDays(-1), timeStamps[1].AddDays(1));
    }
}
