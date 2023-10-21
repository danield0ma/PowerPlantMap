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
            var generatorsOfPowerPlant = await _powerRepository.GetGeneratorNamesOfPowerPlant(powerPlant);
            foreach (var generator in generatorsOfPowerPlant)
            {
                if (generator is not null)
                {
                    var maxPowerOfGenerator = await _powerRepository.GetMaxPowerOfGenerator(generator);
                    var pastActivity = await _powerRepository.GetPastActivity(generator, start, end);
                
                    //calculate
                    var avgPowerOfGenerator = pastActivity.Select(x => x.ActualPower).Average();
                    var avgUsageOfGenerator = Math.Round(avgPowerOfGenerator / maxPowerOfGenerator * 100, 2);

                    DailyStatisticsDto stat = new()
                    {
                        GeneratorName = generator,
                        AverageUsage = avgUsageOfGenerator
                    };
                    statistics.Add(stat);
                    result += $"<li>{generator}: {avgUsageOfGenerator}%</li>";
                }
            }
        }

        result += "</ul></body></html>";
        _emailService.SendEmail("daniel2.doma@gmail.com", "Statistics from PPM", result);
        return statistics;
    }
}
