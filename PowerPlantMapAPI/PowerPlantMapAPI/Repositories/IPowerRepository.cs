using PowerPlantMapAPI.Models.DTO;

namespace PowerPlantMapAPI.Repositories;

public interface IPowerRepository
{
    Task<List<string>> GetPowerPlantNames();
    Task<List<string>> GetGeneratorNames();
    Task<List<string?>> GetGeneratorNamesOfPowerPlant(string powerPlant);
    Task<List<PowerPlantDataDto>> GetDataOfPowerPlants();
    Task<PowerPlantDataDto> GetDataOfPowerPlant(string id);
    Task<List<PowerPlantDetailsDto>> GetPowerPlantDetails(string id);
    Task<List<DateTime>> GetLastDataTime();
    Task<List<PastActivityDto>> GetPastActivity(string? generator, DateTime start, DateTime end);
    Task AddPastActivity(string generatorId, DateTime start, int power);
}