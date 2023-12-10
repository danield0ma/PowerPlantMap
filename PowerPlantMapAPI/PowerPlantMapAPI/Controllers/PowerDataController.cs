using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PowerPlantMapAPI.Data;
using PowerPlantMapAPI.Services;

namespace PowerPlantMapAPI.Controllers;

/// <summary>
/// Production data management
/// </summary>
[EnableCors]
[Route("api/[controller]/[action]")]
[ApiController]
public class PowerDataController : ControllerBase
{
    private readonly IPowerDataService _powerDataService;
    
    /// <summary>
    /// Instantiate the dependencies
    /// </summary>
    public PowerDataController(IPowerDataService powerDataService)
    {
        _powerDataService = powerDataService;
    }

    /// <summary>
    /// Power plant data for the map
    /// </summary>
    /// <returns>List of power plant data</returns>
    [HttpGet("")]
    public async Task<ActionResult<IEnumerable<PowerPlantBasicsModel>>> GetPowerPlantBasics()
    {
        return await _powerDataService.GetPowerPlantBasics();
    }

    /// <summary>
    /// Power plant properties and its detailed production data in the given period
    /// </summary>
    /// <param name="id">Id of the power plant</param>
    /// <param name="date">Optional, one day for the interval</param>
    /// <param name="start">Optional, start of a custom time interval</param>
    /// <param name="end">Optional, end of a custom time interval</param>
    /// <returns>Object of plant properties and its detailed production data</returns>
    [HttpGet("")]
    public async Task<ActionResult<PowerPlantDetailsModel>> GetDetailsOfPowerPlant(string id, DateTime? date = null, DateTime? start = null, DateTime? end = null)
    {
        return await _powerDataService.GetDetailsOfPowerPlant(id, date, start, end);
    }

    /// <summary>
    /// Aggregated power plant production data in the given period
    /// </summary>
    /// <param name="id">Id of the power plant</param>
    /// <param name="date">Optional, one day for the interval</param>
    /// <param name="start">Optional, start of a custom time interval</param>
    /// <param name="end">Optional, end of a custom time interval</param>
    /// <returns>Object of aggregated plant production data</returns>
    [HttpGet("")]
    public async Task<IEnumerable<PowerOfPowerPlantModel>> GetPowerOfPowerPlant(string id, DateTime? date = null, DateTime? start = null, DateTime? end = null)
    {
        return await _powerDataService.GetPowerOfPowerPlant(id, date, start, end);
    }

    /// <summary>
    /// Aggregated production data in the given period of every power plant
    /// </summary>
    /// <param name="date">Optional, one day for the interval</param>
    /// <param name="start">Optional, start of a custom time interval</param>
    /// <param name="end">Optional, end of a custom time interval</param>
    /// <returns>Object of aggregated plant production data of every power plant</returns>
    [HttpGet("")]
    public async Task<PowerOfPowerPlantsModel> GetPowerOfPowerPlants(DateTime? date = null, DateTime? start = null, DateTime? end = null)
    {
        return await _powerDataService.GetPowerOfPowerPlants(date, start, end);
    }

    /// <summary>
    /// Data query from the API in the given time period
    /// </summary>
    /// <param name="start">Optional, start of a custom time interval</param>
    /// <param name="end">Optional, end of a custom time interval</param>
    /// <returns>The time interval within which data was queried</returns>
    [HttpGet("")]
    public async Task<string> QueryDataFromTheApi(DateTime? start = null, DateTime? end = null)
    {
        return await _powerDataService.QueryDataFromTheApi(start, end);
    }
}