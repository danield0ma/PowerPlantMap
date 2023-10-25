using PowerPlantMapAPI.Models.DTO;

namespace ManagementAPI.Repositories;

public interface IPowerPlantRepository
{
    Task<IEnumerable<PowerPlantDataDto?>> Get();
}