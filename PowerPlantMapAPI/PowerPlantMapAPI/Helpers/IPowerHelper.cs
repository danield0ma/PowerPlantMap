using PowerPlantMapAPI.Models.DTO;
using System.Xml;

namespace PowerPlantMapAPI.Helpers
{
    public interface IPowerHelper
    {
        Task<XmlDocument> APIquery(string DocumentType, string PeriodStart, string PeriodEnd);
        Task<List<GeneratorPowerDTO>> GetGeneratorPower(string generator, DateTime start, DateTime end);
        Task<List<PowerStampDTO>> GetPowerStampsListOfPowerPlant(string Id, int NumberOfDataPoints, List<DateTime> TimeStamps);
    }
}
