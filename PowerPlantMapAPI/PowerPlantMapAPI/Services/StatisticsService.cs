using PowerPlantMapAPI.Repositories;

namespace PowerPlantMapAPI.Services
{
    public class StatisticsService
    {
        private readonly IPowerRepository _powerRepository;
        private readonly IDateService _dateService;
        
        public StatisticsService(IPowerRepository powerRepository, IDateService dateService)
        {
            _powerRepository = powerRepository;
            _dateService = dateService;
        }
        
        public async Task CreateAndSendDailyStatistics()
        {
            var startAndEndTimeOfDailyStatistics = _dateService.GetStartAndEndTimeOfDailyStatistics();
            var start = startAndEndTimeOfDailyStatistics[0];
            var end = startAndEndTimeOfDailyStatistics[1];

            var powerPlants = await _powerRepository.GetPowerPlants();
            foreach (var powerPlant in powerPlants)
            {
                var generatorsOfPowerPlant = await _powerRepository.GetGeneratorsOfPowerPlant(powerPlant);
                foreach (var generator in generatorsOfPowerPlant)
                {
                    var maxCapacity = await _powerRepository.GetMaxPowerOfGenerator(generator);
                    var pastActivity = await _powerRepository.GetPastActivity(generator, start, end);
                    //calculate
                }
            }
        }
    }
}
