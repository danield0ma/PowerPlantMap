using PowerPlantMapAPI.Data.Dto;

namespace PowerPlantMapAPI.Services;

public interface IPowerPlantService
{
    Task<IEnumerable<PowerPlantDataDto?>> Get();
}