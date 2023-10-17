using PowerPlantMapAPI.Models;

namespace PowerPlantMapAPI.Repositories
{
    public interface IPowerRepository
    {
        Task<List<string>> GetPowerPlants();
        Task<List<string>> GetGenerators();
        Task<int> GetMaxPowerOfGenerator(string generator);
        Task<List<string>> GetGeneratorsOfPowerPlant(string powerPlant);
        Task<List<PowerPlantDataModel>> GetPowerPlantBasics();
        Task<PowerPlantDataModel> GetBasicsOfPowerPlant(string id);
        Task<List<PowerPlantDetailsModel>> GetPowerPlantDetails(string id);
        Task<List<DateTime>> GetLastDataTime();
        Task<List<PastActivityModel>> GetPastActivity(string generator, DateTime start, DateTime end);
        Task AddPastActivity(string generatorId, DateTime start, int power);
    }
}
