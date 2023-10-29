using PowerPlantMapAPI.Data.Dto;

namespace PowerPlantMapAPI.Services;

public interface IPowerPlantService
{
    Task<IEnumerable<PowerPlantDataDto?>> Get();
    Task<PowerPlantDataDto?> GetById(string id);
    Task<bool> AddPowerPlant(CreatePowerPlantDto powerPlantToBeAdded);
    Task<bool> DeletePowerPlant(string id);
}