using PowerPlantMapAPI.Models.DTO;
using PowerPlantMapAPI.Repositories;

namespace PowerPlantMapAPI.Services;

public class StatisticsService : IStatisticsService
{
    private readonly IPowerRepository _powerRepository;
    private readonly IDateService _dateService;
    private readonly IEmailService _emailService;
    
    public StatisticsService(IPowerRepository powerRepository, IDateService dateService, IEmailService emailService)
    {
        _powerRepository = powerRepository;
        _dateService = dateService;
        _emailService = emailService;
    }
    
    public async Task<List<DailyStatisticsDto>> CreateAndSendDailyStatistics()
    {
        List<DailyStatisticsDto> statistics = new();
        var result = "";
        
        var startAndEndTimeOfDailyStatistics = _dateService.GetStartAndEndTimeOfDailyStatistics();
        var start = startAndEndTimeOfDailyStatistics[0];
        var end = startAndEndTimeOfDailyStatistics[1];

        var powerPlants = await _powerRepository.GetPowerPlants();
        foreach (var powerPlant in powerPlants)
        {
            var generatorsOfPowerPlant = await _powerRepository.GetGeneratorsOfPowerPlant(powerPlant);
            foreach (var generator in generatorsOfPowerPlant)
            {
                var maxPowerOfGenerator = await _powerRepository.GetMaxPowerOfGenerator(generator);
                var pastActivity = await _powerRepository.GetPastActivity(generator, start, end);
                
                //calculate
                var avgPowerOfGenerator = pastActivity.Select(x => x.ActualPower).Average();
                var avgUsageOfGenerator = avgPowerOfGenerator / maxPowerOfGenerator;

                DailyStatisticsDto stat = new()
                {
                    GeneratorName = generator,
                    AverageUsage = avgUsageOfGenerator
                };
                statistics.Add(stat);

                result += "${generator}: ${avgUsageOfGenerator}";
            }
        }
        
        _emailService.SendEmail("daniel2.doma@gmail.com", "Statistics from PPM", result);
        return statistics;
    }
}
