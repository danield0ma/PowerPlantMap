using System.Text;
using PowerPlantMapAPI.Helpers;
using PowerPlantMapAPI.Models.DTO;
using PowerPlantMapAPI.Repositories;

namespace PowerPlantMapAPI.Services;

public class StatisticsService : IStatisticsService
{
    private readonly IPowerRepository _powerRepository;
    private readonly IDateHelper _dateHelper;
    private readonly IEmailService _emailService;
    
    public StatisticsService(IPowerRepository powerRepository, IDateHelper dateHelper, IEmailService emailService)
    {
        _powerRepository = powerRepository;
        _dateHelper = dateHelper;
        _emailService = emailService;
    }
    
    public async Task<List<DailyStatisticsDto>> CreateAndSendDailyStatistics()
    {
        List<DailyStatisticsDto> statistics = new();
        var startAndEndTimeOfDailyStatistics = _dateHelper.GetStartAndEndTimeOfDailyStatistics();
        var date = startAndEndTimeOfDailyStatistics[0].Year + "." +
                        startAndEndTimeOfDailyStatistics[0].Month + "." +
                        startAndEndTimeOfDailyStatistics[0].Day;
        var start = startAndEndTimeOfDailyStatistics[0];
        var end = startAndEndTimeOfDailyStatistics[1];
        var result = new StringBuilder($"<html><body><h3>Napi erőműstatisztika - {date}</h3><ul>");

        // var powerPlants = await _powerRepository.GetPowerPlantNames();
        var powerPlants = await _powerRepository.GetDataOfPowerPlants();
        foreach (var powerPlant in powerPlants.Where(x => x.IsCountry == false).ToList())
        {
            if (powerPlant.PowerPlantId is null) continue;
            var powerPlantDetails = await _powerRepository.GetPowerPlantDetails(powerPlant.PowerPlantId);
            
            var powerPlantMaxPower = 0;
            var powerPlantCurrentAvgPower = 0.0;
            var powerPlantGeneratedEnergy = 0.0;
            var blocData = new StringBuilder("<ul>");
            
            var blocs = powerPlantDetails.
                GroupBy(x => x.BlocId).
                Select(group => group.First()).
                ToList();
            foreach (var bloc in blocs)
            {
                var blocMaxPower = 0;
                var blocCurrentAvgPower = 0.0;
                var blocGeneratedEnergy = 0.0;
                var generatorData = new StringBuilder("<ul>");

                var generators = powerPlantDetails.
                    Where(x => x.BlocId == bloc.BlocId).ToList();
                foreach (var generator in generators)
                {
                    if (generator.GeneratorId == null) continue;
                    var maxPowerOfGenerator = await _powerRepository.GetMaxPowerOfGenerator(generator.GeneratorId);
                    var pastActivity = await _powerRepository.GetPastActivity(generator.GeneratorId, start, end);
                    
                    var avgPowerOfGenerator = Math.Round(pastActivity.Select(x => x.ActualPower).Average(), 2);
                    var generatedEnergyByGenerator = Math.Round(pastActivity.Sum(activity => activity.ActualPower * 0.25), 2);
                    var avgUsageOfGenerator = Math.Round(avgPowerOfGenerator / maxPowerOfGenerator * 100, 2);

                    DailyStatisticsDto stat = new()
                    {
                        GeneratorName = generator.GeneratorId,
                        MaxPower = maxPowerOfGenerator,
                        AveragePower = avgPowerOfGenerator,
                        GeneratedEnergy = generatedEnergyByGenerator,
                        AverageUsage = avgUsageOfGenerator
                    };
                    statistics.Add(stat);
                    generatorData.Append($"<li>{generator.GeneratorId}: {avgUsageOfGenerator}% - ");
                    generatorData.Append($"{avgPowerOfGenerator}MW/{maxPowerOfGenerator}MW -> {generatedEnergyByGenerator}MWh</li>");
                    
                    blocMaxPower += maxPowerOfGenerator;
                    blocCurrentAvgPower += Math.Round(avgPowerOfGenerator, 2);
                    blocGeneratedEnergy += Math.Round(generatedEnergyByGenerator, 2);
                }

                var avgUsageOfBloc = Math.Round(blocCurrentAvgPower / blocMaxPower * 100, 2);
                blocData.Append($"<li>{bloc.BlocId}: {avgUsageOfBloc}% - ");
                blocData.Append($"{Math.Round(blocCurrentAvgPower, 2)}MW/{blocMaxPower}MW -> {blocGeneratedEnergy}MWh</li>");
                if (generators.Count > 1)
                {
                    blocData.Append($"{generatorData}</ul>");
                }
                
                powerPlantMaxPower += blocMaxPower;
                powerPlantCurrentAvgPower += Math.Round(blocCurrentAvgPower, 2);
                powerPlantGeneratedEnergy += Math.Round(blocGeneratedEnergy, 2);
            }
            
            var avgUsageOfPowerPlant = Math.Round(powerPlantCurrentAvgPower / powerPlantMaxPower * 100, 2);
            result.Append($"<li>{powerPlant.Description}: {avgUsageOfPowerPlant}% - ");
            result.Append($"{Math.Round(powerPlantCurrentAvgPower, 2)}MW/{powerPlantMaxPower}MW -> {powerPlantGeneratedEnergy}MWh</li>");
            if (blocs.Count > 1 ||
                powerPlantDetails.Where(x => x.BlocId == blocs[0].BlocId).ToList().Count > 1)
            {
                result.Append($"{blocData}</ul>");
            }
        }
        result.Append($"</ul><h3>Import-Export statisztika - {date}</h3><ul>");
        foreach (var powerPlant in powerPlants.Where(x => x.IsCountry).ToList())
        {
            result.Append($"<li>{powerPlant.PowerPlantId}</li>");
        }

        result.Append("</ul></body></html>");
        _emailService.SendEmail("daniel2.doma@gmail.com",
                            $"Napi erőműstatisztika - {date}",
                            result.ToString());
        return statistics;
    }

    // private async Task UpdateMaxPowerCapacitiesOfPowerPlants()
    // {
    //     
    // }
}
