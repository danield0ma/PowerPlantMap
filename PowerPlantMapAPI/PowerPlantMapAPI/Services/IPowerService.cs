using Microsoft.AspNetCore.Mvc;
using PowerPlantMapAPI.Models;
using PowerPlantMapAPI.Models.DTO;

namespace PowerPlantMapAPI.Services
{
    public interface IPowerService
    {
        Task<ActionResult<IEnumerable<FeatureDTO>>> GetPowerPlantBasics();
        Task<PowerPlantDataModel> GetBasicsOfPowerPlant(string id);
        Task<ActionResult<PowerPlantDetailsDTO>> GetDetailsOfPowerPlant(string id, DateTime? date = null);
        Task<PowerOfPowerPlantsDTO> GetPowerOfPowerPlants(DateTime? date = null, DateTime? Start = null, DateTime? End = null);
        Task<string> InitData(DateTime? periodStart, DateTime? periodEnd);
    }
}
