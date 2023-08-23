using PowerPlantMapAPI.Models.DTO;

namespace PowerPlantMapAPI.Helpers
{
    public interface IPowerHelper
    {
        Task<List<GeneratorPowerDTO>> GetGeneratorPower(string generator, DateTime start, DateTime end);
        Task<List<PowerStampDTO>> GetPowerStampsListOfPowerPlant(string Id, int NumberOfDataPoints, List<DateTime> TimeStamps);
    }
}
