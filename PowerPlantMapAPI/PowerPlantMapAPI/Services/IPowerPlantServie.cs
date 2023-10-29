using PowerPlantMapAPI.Data;
using PowerPlantMapAPI.Data.Dto;

namespace PowerPlantMapAPI.Services;

public interface IPowerPlantService
{
    Task<IEnumerable<PowerPlantModel?>> Get();
    Task<PowerPlantModel?> GetById(string id);
    Task<bool> AddPowerPlant(CreatePowerPlantDto powerPlantToBeAdded);
    Task<bool> DeletePowerPlant(string id);
}