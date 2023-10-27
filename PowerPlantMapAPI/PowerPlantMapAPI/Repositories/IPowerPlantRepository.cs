using PowerPlantMapAPI.Data.Dto;

namespace PowerPlantMapAPI.Repositories;

public interface IPowerPlantRepository
{
    Task<IEnumerable<PowerPlantDataDto?>> Get();
    Task<PowerPlantDataDto?> GetById(string id);
    Task<bool> AddPowerPlant(PowerPlantDataDto powerPlantToBeAdded);
    Task<bool> DeletePowerPlant(string id);
}