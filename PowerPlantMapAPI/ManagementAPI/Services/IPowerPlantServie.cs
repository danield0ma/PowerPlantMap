using PowerPlantMapAPI.Models.DTO;

namespace ManagementAPI.Services;

public interface IPowerPlantService
{
    Task<IEnumerable<PowerPlantDataDto?>> Get();
}