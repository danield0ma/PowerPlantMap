using Microsoft.AspNetCore.Mvc;
using PowerPlantMapAPI.Models;

namespace PowerPlantMapAPI.Services;

public interface IPowerService
{
    Task<ActionResult<IEnumerable<PowerPlantBasicsModel>>> GetPowerPlantBasics();
    Task<ActionResult<PowerPlantDetailsModel>> GetDetailsOfPowerPlant(string id, DateTime? date = null, DateTime? start = null, DateTime? end = null);
    Task<IEnumerable<PowerOfPowerPlantModel>> GetPowerOfPowerPlant(string id, DateTime? date = null, DateTime? start = null, DateTime? end = null);
    Task<PowerOfPowerPlantsModel> GetPowerOfPowerPlants(DateTime? date = null, DateTime? start = null, DateTime? end = null);
    Task<string> InitData(DateTime? periodStart, DateTime? periodEnd);
}
