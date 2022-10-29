using PowerPlantMapAPI.Models.DTO;

namespace PowerPlantMapAPI.Services
{
    public interface IPowerService
    {
        Task<CurrentLoadDTO> GetCurrentLoad(string periodStart, string periodEnd);
        Task<IEnumerable<CurrentLoadDTO>> GetLoadHistory(DateTime periodStart, DateTime periodEnd);
        string EditTime(DateTime start);
        Task<List<DateTime>> GetStartAndEnd(bool initData);
        Task<IEnumerable<PowerDTO>> getPPData(string docType, string periodStart, string periodEnd);
        Task<IEnumerable<PowerDTO>> getImportData(bool export, string periodStart, string periodEnd);
        Task<string> InitData();
    }
}
