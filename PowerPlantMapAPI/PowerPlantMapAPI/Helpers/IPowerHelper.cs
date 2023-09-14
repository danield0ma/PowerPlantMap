using PowerPlantMapAPI.Models.DTO;
using System.Xml;

namespace PowerPlantMapAPI.Helpers
{
    public interface IPowerHelper
    {
        Task<string> APIquery(string documentType, DateTime start, DateTime end, string? inDomain = null, string? outDomain = null);
        Task<List<GeneratorPowerDTO>> GetGeneratorPower(string generator, DateTime start, DateTime end);
        Task<List<PowerStampDTO>> GetPowerStampsListOfPowerPlant(string Id, int NumberOfDataPoints, List<DateTime> TimeStamps);
    }
}
