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
}