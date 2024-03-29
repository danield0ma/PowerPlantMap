﻿using PowerPlantMapAPI.Data;
using PowerPlantMapAPI.Data.Dto;
using PowerPlantMapAPI.Helpers;
using PowerPlantMapAPI.Repositories;

namespace PowerPlantMapAPI.Services;

public class StatisticsService : IStatisticsService
{
    private readonly IPowerDataRepository _powerDataRepository;
    private readonly IDateHelper _dateHelper;
    
    public StatisticsService(IPowerDataRepository powerDataRepository, IDateHelper dateHelper)
    {
        _powerDataRepository = powerDataRepository;
        _dateHelper = dateHelper;
    }

    public async Task<PowerPlantStatisticsDtoWrapper> GenerateDailyPowerPlantStatistics(DateTime? day = null,
        DateTime? start = null, DateTime? end = null)
    {
        List<PowerPlantStatisticsDto> data = new();

        List<DateTime> startAndEndTimeOfDailyStatistics;
        if (day is null && start is null && end is null)
        {
            startAndEndTimeOfDailyStatistics = _dateHelper.GetStartAndEndTimeOfDailyStatistics();
        }
        else
        {
            startAndEndTimeOfDailyStatistics = await _dateHelper.HandleWhichDateFormatIsBeingUsed(day, start, end);
        }

        var startTime = startAndEndTimeOfDailyStatistics[0];
        var endTime = startAndEndTimeOfDailyStatistics[1];

        var powerPlants = await _powerDataRepository.GetDataOfPowerPlants();
        foreach (var powerPlant in powerPlants.Where(x => x.IsCountry == false).ToList())
        {
            if (powerPlant.PowerPlantId is null) continue;
            var powerPlantDetails = await _powerDataRepository.GetPowerPlantDetails(powerPlant.PowerPlantId);

            var blocs = powerPlantDetails.GroupBy(x => x.BlocId).Select(group => group.First()).ToList();
            foreach (var bloc in blocs)
            {
                var generators = powerPlantDetails.Where(x => x.BlocId == bloc.BlocId).ToList();
                foreach (var generator in generators)
                {
                    if (generator.GeneratorId == null) continue;
                    var maxPowerOfGenerator = await _powerDataRepository.GetMaxPowerOfGenerator(generator.GeneratorId);
                    var pastActivity =
                        await _powerDataRepository.GetPastActivity(generator.GeneratorId, startTime, endTime);

                    var avgPowerOfGenerator = Math.Round(pastActivity.Select(x => x.ActualPower).Average(), 3);
                    var generatedEnergyByGenerator =
                        Math.Round(pastActivity.Sum(activity => activity.ActualPower * 0.25), 3);
                    var avgUsageOfGenerator = Math.Round(avgPowerOfGenerator / maxPowerOfGenerator * 100, 3);

                    PowerPlantStatisticsDto stat = new()
                    {
                        PowerPlantId = powerPlant.PowerPlantId,
                        PowerPlantName = powerPlant.Name,
                        PowerPlantDescription = powerPlant.Description,
                        Image = powerPlant.Image,
                        BlocId = bloc.BlocId,
                        GeneratorId = generator.GeneratorId,
                        MaxPower = maxPowerOfGenerator,
                        AveragePower = avgPowerOfGenerator,
                        GeneratedEnergy = generatedEnergyByGenerator,
                        AverageUsage = avgUsageOfGenerator
                    };
                    data.Add(stat);
                }
            }
        }
        
        var gasPowerPlants = new string[] { "DME", "GNY", "CSP", "KF", "KP" };
        var gasEnergy = 0.0; var gasPower = 0.0;
        foreach (var gasPowerPlant in gasPowerPlants)
        {
            gasEnergy += data.Where(x => x.PowerPlantId == gasPowerPlant).Select(x => x.GeneratedEnergy).Sum();
            gasPower += data.Where(x => x.PowerPlantId == gasPowerPlant).Select(x => x.AveragePower).Sum();
        }
        
        foreach (var t in data)
        {
            if (t.PowerPlantId != "GAS") continue;
            t.GeneratedEnergy = Math.Round(t.GeneratedEnergy -= gasEnergy, 3);
            t.AveragePower = Math.Round(t.AveragePower -= gasPower, 3);
            t.AverageUsage = Math.Round(t.AveragePower / t.MaxPower * 100, 3);
        }
        
        PowerPlantStatisticsDtoWrapper statistics = new()
        {
            Start = startTime,
            End = endTime,
            Data = data
        };
        return statistics;
    }

    public async Task<IEnumerable<CompactPowerPlantStatistics>> GenerateCompactPowerPlantStatistics(DateTime? day = null,
        DateTime? start = null, DateTime? end = null)
    {
        var compactPowerPlantStatistics = new List<CompactPowerPlantStatistics>();
        
        var powerPlants = await _powerDataRepository.GetDataOfPowerPlants();
        var powerPlantStatistics = await GenerateDailyPowerPlantStatistics(day, start, end);
        var filteredPowerPlants = powerPlants.Where(x => x.IsCountry == false).ToList();
        var generatedEnergyByPowerPlants = new List<double>();

        foreach (var powerPlant in filteredPowerPlants)
        {
            var statisticsOfCurrentPowerPlant = powerPlantStatistics.Data?
                .Where(x => x.PowerPlantId == powerPlant.PowerPlantId);
            
            var powerPlantMaxPower = statisticsOfCurrentPowerPlant.Select(x => x.MaxPower).Sum();
            var powerPlantCurrentAvgPower = statisticsOfCurrentPowerPlant.Select(x => x.AveragePower).Sum();
            var powerPlantGeneratedEnergy = statisticsOfCurrentPowerPlant.Select(x => x.GeneratedEnergy).Sum();
            var avgUsageOfPowerPlant = Math.Round(powerPlantCurrentAvgPower / powerPlantMaxPower * 100, 3);
            
            compactPowerPlantStatistics.Add(new CompactPowerPlantStatistics()
            {
                PowerPlantId = powerPlant.PowerPlantId,
                PowerPlantName = powerPlant.Name,
                PowerPlantDescription = powerPlant.Description,
                Image = powerPlant.Image,
                MaxPower = powerPlantMaxPower,
                AveragePower = powerPlantCurrentAvgPower,
                GeneratedEnergy = powerPlantGeneratedEnergy,
                AverageUsage = avgUsageOfPowerPlant
            });
        }

        return compactPowerPlantStatistics;
    }

    public async Task<CountryStatisticsDtoWrapper> GenerateDailyCountryStatistics(DateTime? day = null, DateTime? start = null, DateTime? end = null)
    {
        List<CountryStatisticsDto> data = new();
        
        List<DateTime> startAndEndTimeOfDailyStatistics;
        if (day is null && start is null && end is null)
        {
            startAndEndTimeOfDailyStatistics = _dateHelper.GetStartAndEndTimeOfDailyStatistics();
        }
        else
        {
            startAndEndTimeOfDailyStatistics = await _dateHelper.HandleWhichDateFormatIsBeingUsed(day, start, end);
        }
        var startTime = startAndEndTimeOfDailyStatistics[0];
        var endTime = startAndEndTimeOfDailyStatistics[1];
        
        var powerPlants = await _powerDataRepository.GetDataOfPowerPlants();
        foreach (var country in powerPlants.Where(x => x.IsCountry).ToList())
        {
            if (country.PowerPlantId is null) continue;
            var generator = await _powerDataRepository.GetGeneratorNamesOfPowerPlant(country.PowerPlantId);
            var pastActivity = await _powerDataRepository.GetPastActivity(generator[0], startTime, endTime);
            var importedEnergy = Math.Round(pastActivity.
                Where(x => x.ActualPower > 0).
                Sum(x => x.ActualPower * 0.25), 3);
            var exportedEnergy = Math.Abs(Math.Round(pastActivity.
                Where(x => x.ActualPower < 0).
                Sum(x => x.ActualPower * 0.25), 3));
            var stat = new CountryStatisticsDto()
            {
                CountryId = country.PowerPlantId,
                CountryName = country.Name,
                Image = country.Image,
                ImportedEnergy = importedEnergy,
                ExportedEnergy = exportedEnergy
            };
            data.Add(stat);
        }
        
        CountryStatisticsDtoWrapper statistics = new()
        {
            Start = startTime,
            End = endTime,
            Data = data
        };
        return statistics;
    }
}
