using System.Globalization;
using System.Text;
using PowerPlantMapAPI.Helpers;
using PowerPlantMapAPI.Models.DTO;
using PowerPlantMapAPI.Repositories;

namespace PowerPlantMapAPI.Services;

public class StatisticsService : IStatisticsService
{
    private readonly IPowerRepository _powerRepository;
    private readonly IDateHelper _dateHelper;
    
    public StatisticsService(IPowerRepository powerRepository, IDateHelper dateHelper)
    {
        _powerRepository = powerRepository;
        _dateHelper = dateHelper;
    }
    
    public async Task<List<PowerPlantStatisticsDto>> GenerateDailyPowerPlantStatistics()
    {
        List<PowerPlantStatisticsDto> statistics = new();
        var startAndEndTimeOfDailyStatistics = _dateHelper.GetStartAndEndTimeOfDailyStatistics();
        var date = startAndEndTimeOfDailyStatistics[0].Year + "." +
                        startAndEndTimeOfDailyStatistics[0].Month + "." +
                        startAndEndTimeOfDailyStatistics[0].Day;
        var start = startAndEndTimeOfDailyStatistics[0];
        var end = startAndEndTimeOfDailyStatistics[1];
        
        var powerPlants = await _powerRepository.GetDataOfPowerPlants();
        foreach (var powerPlant in powerPlants.Where(x => x.IsCountry == false).ToList())
        {
            if (powerPlant.PowerPlantId is null) continue;
            var powerPlantDetails = await _powerRepository.GetPowerPlantDetails(powerPlant.PowerPlantId);
            
            var blocs = powerPlantDetails.
                GroupBy(x => x.BlocId).
                Select(group => group.First()).
                ToList();
            foreach (var bloc in blocs)
            {
                var generators = powerPlantDetails.
                    Where(x => x.BlocId == bloc.BlocId).ToList();
                foreach (var generator in generators)
                {
                    if (generator.GeneratorId == null) continue;
                    var maxPowerOfGenerator = await _powerRepository.GetMaxPowerOfGenerator(generator.GeneratorId);
                    var pastActivity = await _powerRepository.GetPastActivity(generator.GeneratorId, start, end);
                    
                    var avgPowerOfGenerator = Math.Round(pastActivity.Select(x => x.ActualPower).Average(), 3);
                    var generatedEnergyByGenerator = Math.Round(pastActivity.Sum(activity => activity.ActualPower * 0.25), 3);
                    var avgUsageOfGenerator = Math.Round(avgPowerOfGenerator / maxPowerOfGenerator * 100, 3);

                    PowerPlantStatisticsDto stat = new()
                    {
                        PowerPlantId = powerPlant.PowerPlantId,
                        BlocId = bloc.BlocId,
                        GeneratorId = generator.GeneratorId,
                        MaxPower = maxPowerOfGenerator,
                        AveragePower = avgPowerOfGenerator,
                        GeneratedEnergy = generatedEnergyByGenerator,
                        AverageUsage = avgUsageOfGenerator
                    };
                    statistics.Add(stat);
                }
            }
        }
        
        return statistics;
    }

    public async Task<List<CountryStatisticsDto>> GenerateDailyCountryStatistics()
    {
        List<CountryStatisticsDto> statistics = new();
        var startAndEndTimeOfDailyStatistics = _dateHelper.GetStartAndEndTimeOfDailyStatistics();
        var date = startAndEndTimeOfDailyStatistics[0].Year + "." +
                   startAndEndTimeOfDailyStatistics[0].Month + "." +
                   startAndEndTimeOfDailyStatistics[0].Day;
        var start = startAndEndTimeOfDailyStatistics[0];
        var end = startAndEndTimeOfDailyStatistics[1];
        
        var powerPlants = await _powerRepository.GetDataOfPowerPlants();
        foreach (var country in powerPlants.Where(x => x.IsCountry).ToList())
        {
            if (country.PowerPlantId is null) continue;
            var generator = await _powerRepository.GetGeneratorNamesOfPowerPlant(country.PowerPlantId);
            var pastActivity = await _powerRepository.GetPastActivity(generator[0], start, end);
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
                ImportedEnergy = importedEnergy,
                ExportedEnergy = exportedEnergy
            };
            statistics.Add(stat);
        }
        return statistics;
    }

    // private async Task UpdateMaxPowerCapacitiesOfPowerPlants()
    // {
    //     
    // }
}
