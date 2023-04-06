using PowerPlantMapAPI.Models;
using PowerPlantMapAPI.Models.DTO;

namespace PowerPlantMapAPI.Repositories
{
    public interface IPowerRepository
    {
        Task<List<PowerPlantDataDTO>> QueryPowerPlantBasics();
        Task<List<PowerPlantDataDTO>> QueryBasicsOfPowerPlant(string id);
        Task<List<PowerPlantDetailsDTO>> QueryPowerPlantDetails(string id);        
        Task<List<PastActivityModel>> QueryPastActivity(string generator, DateTime start, DateTime end);
        Task<List<string>> QueryPowerPlants();
        Task<List<string>> QueryGeneratorsOfPowerPlant(string PowerPlant);
        Task<List<DateTime>> QueryLastDataTime();
        Task<List<string>> QueryGenerators();
        Task InsertData(string GID, DateTime start, int power);
    }
}
