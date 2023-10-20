using PowerPlantMapAPI.Models;
using PowerPlantMapAPI.Models.DTO;

namespace PowerPlantMapAPI.Helpers;

public interface IPowerHelper
{
    Task<string> ApiQuery(string documentType, DateTime start, DateTime end, string? inDomain = null, string? outDomain = null);
    Task<List<GeneratorPowerDto>?> GetGeneratorPower(string? generator, DateTime start, DateTime end);
    Task<List<PowerOfPowerPlantModel>> GetPowerStampsListOfPowerPlant(string id, int numberOfDataPoints, List<DateTime> timeStamps);
}