using PowerPlantMapAPI.Models;

namespace PowerPlantMapAPI.Repositories
{
    public interface IPowerRepository
    {
        Task<List<PowerPlantDataModel>> QueryPowerPlantBasics();
        Task<PowerPlantDataModel> QueryBasicsOfPowerPlant(string id);
        Task<List<PowerPlantDetailsModel>> QueryPowerPlantDetails(string id);        
        Task<List<PastActivityModel>> QueryPastActivity(string generator, DateTime start, DateTime end);
        Task<List<string>> QueryPowerPlants();
        Task<List<string>> QueryGeneratorsOfPowerPlant(string PowerPlant);
        Task<List<DateTime>> QueryLastDataTime();
        Task<List<string>> QueryGenerators();
        Task InsertData(string GID, DateTime start, int power);
    }
}
