using Microsoft.AspNetCore.Mvc;
using PowerPlantMapAPI.Models;
using PowerPlantMapAPI.Models.DTO;

namespace PowerPlantMapAPI.Services
{
    public interface IPowerService
    {
        Task<CurrentLoadDTO> GetCurrentLoad(string periodStart, string periodEnd);
        Task<IEnumerable<CurrentLoadDTO>> GetLoadHistory(DateTime periodStart, DateTime periodEnd);
        Task<ActionResult<IEnumerable<FeatureModel>>> getPowerPlantBasics();
        Task<ActionResult<PowerPlantDetailsModel>> getDetailsOfPowerPlant(string id, DateTime? date = null);
        Task<PowerOfPowerPlantsModel> GetPowerOfPowerPlants(DateTime? date = null);
        Task<BasicsOfPowerPlantDTO> GetBasicsOfPowerPlant(string id);
        Task<List<int>> GetGeneratorPower(string generator, DateTime start, DateTime end);
        Task<string> CheckData(List<DateTime> TimeStamps);
        Task<IEnumerable<PowerDTO>> getPPData(string docType, string periodStart, string periodEnd);
        Task<IEnumerable<PowerDTO>> getImportData(bool export, string periodStart, string periodEnd);
        Task<string> InitData(DateTime? periodStart, DateTime? periodEnd);
    }
}
