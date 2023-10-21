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
        var result = "<html><body><ul>";
        
        var startAndEndTimeOfDailyStatistics = _dateHelper.GetStartAndEndTimeOfDailyStatistics();
        var start = startAndEndTimeOfDailyStatistics[0];
        var end = startAndEndTimeOfDailyStatistics[1];

        var powerPlants = await _powerRepository.GetPowerPlantNames();
        foreach (var powerPlant in powerPlants)
        {
            var powerPlantMaxPower = 0;
            var powerPlantCurrentAvgPower = 0.0;
            var blocData = "<ul>";

            var powerPlantDetails = await _powerRepository.GetPowerPlantDetails(powerPlant);
            var blocs = powerPlantDetails.
                GroupBy(x => x.BlocId).
                Select(group => group.First()).
                ToList();
            foreach (var bloc in blocs)
            {
                var blocMaxPower = 0;
                var blocCurrentAvgPower = 0.0;
                var generatorData = "<ul>";

                var generators = powerPlantDetails.Where(x => x.BlocId == bloc.BlocId);
                foreach (var generator in generators)
                {
                    if (generator is null) continue;
                    var maxPowerOfGenerator = await _powerRepository.GetMaxPowerOfGenerator(generator.GeneratorId);
                    blocMaxPower += maxPowerOfGenerator;
                    var pastActivity = await _powerRepository.GetPastActivity(generator.GeneratorId, start, end);

                    var avgPowerOfGenerator = pastActivity.Select(x => x.ActualPower).Average();
                    blocCurrentAvgPower += avgPowerOfGenerator;
                    var avgUsageOfGenerator = Math.Round(avgPowerOfGenerator / maxPowerOfGenerator * 100, 2);

                    DailyStatisticsDto stat = new()
                    {
                        GeneratorName = generator.GeneratorId,
                        AverageUsage = avgUsageOfGenerator
                    };
                    statistics.Add(stat);
                    generatorData += $"<li>{generator.GeneratorId}: {avgUsageOfGenerator}%</li>";
                }

                powerPlantMaxPower += blocMaxPower;
                powerPlantCurrentAvgPower += blocCurrentAvgPower;
                
                var avgUsageOfBloc = Math.Round(blocCurrentAvgPower / blocMaxPower * 100, 2);
                blocData += $"<li>{bloc.BlocId}: {avgUsageOfBloc}%</li>";
                if (generators.Count() > 1)
                {
                    blocData += $"{generatorData}</ul>";
                }
            }
            
            var avgUsageOfPowerPlant = Math.Round(powerPlantCurrentAvgPower / powerPlantMaxPower * 100, 2);
            result += $"<li>{powerPlant}: {avgUsageOfPowerPlant}%</li>";
            if (blocs.Count() > 1)
            {
                result += $"{blocData}</ul>";
            }
        }

        result += "</ul></body></html>";
        _emailService.SendEmail("daniel2.doma@gmail.com", "Statistics from PPM", result);
        return statistics;
    }
}
