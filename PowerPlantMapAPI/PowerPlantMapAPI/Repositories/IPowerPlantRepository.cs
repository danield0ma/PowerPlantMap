using PowerPlantMapAPI.Data.Dto;

namespace PowerPlantMapAPI.Repositories;

public interface IPowerPlantRepository
{
    Task<IEnumerable<PowerPlantDataDto?>> Get();
}