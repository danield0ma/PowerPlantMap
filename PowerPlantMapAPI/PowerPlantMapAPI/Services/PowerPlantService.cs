using PowerPlantMapAPI.Repositories;
using PowerPlantMapAPI.Data.Dto;

namespace PowerPlantMapAPI.Services;

public class PowerPlantService : IPowerPlantService
{
    private readonly IPowerPlantRepository _powerPlantRepository;

    public PowerPlantService(IPowerPlantRepository powerPlantRepository)
    {
        _powerPlantRepository = powerPlantRepository;
    }
    
    public async Task<IEnumerable<PowerPlantDataDto?>> Get()
    {
        return await _powerPlantRepository.Get();
    }
    
    public async Task<PowerPlantDataDto?> GetById(string id)
    {
        return await _powerPlantRepository.GetById(id);
    }
    
    public async Task<bool> AddPowerPlant(PowerPlantDataDto powerPlantToBeAdded)
    {
        return await _powerPlantRepository.AddPowerPlant(powerPlantToBeAdded);
    }
    
    public async Task<bool> DeletePowerPlant(string id)
    {
        return await _powerPlantRepository.DeletePowerPlant(id);
    }
    
}