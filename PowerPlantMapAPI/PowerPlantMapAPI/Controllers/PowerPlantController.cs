using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PowerPlantMapAPI.Data;
using PowerPlantMapAPI.Data.Dto;
using PowerPlantMapAPI.Services;

namespace PowerPlantMapAPI.Controllers;

/// <summary>
/// Manage power plant data
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class PowerPlantController : ControllerBase
{
    private readonly IPowerPlantService _powerPlantService;

    /// <summary>
    /// Instantiate the dependencies
    /// </summary>
    public PowerPlantController(IPowerPlantService powerPlantService)
    {
        _powerPlantService = powerPlantService;
    }
    
    /// <summary>
    /// Get all power plants
    /// </summary>
    /// <returns>List of all power plants</returns>
    [HttpGet]
    [Authorize(Roles = "user, admin")]
    public async Task<ActionResult<IEnumerable<PowerPlantModel?>?>> Get()
    {
        try
        {
            var powerPlants = await _powerPlantService.Get();
            return Ok(powerPlants);
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e);
            return NoContent();
        }
    }
    
    /// <summary>
    /// Get power plant by id
    /// </summary>
    /// <param name="id">id of the power plant</param>
    /// <returns>Object of the given power plant</returns>
    [HttpGet]
    [Authorize(Roles = "user, admin")]
    public async Task<ActionResult<PowerPlantModel?>> GetById(string id)
    {
        try
        {
            var powerPlant = await _powerPlantService.GetById(id);
            return powerPlant is null ? NoContent() : Ok(powerPlant);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return NoContent();
        }
    }
    
    /// <summary>
    /// Add new power plant
    /// </summary>
    /// <param name="createPowerPlantDto">Object of the new power plant</param>
    /// <returns>Result of the process</returns>
    [HttpPost]
    [Authorize(Roles = "user, admin")]
    public async Task<ActionResult> AddPowerPlant(CreatePowerPlantDto createPowerPlantDto)
    {
        try
        {
            await _powerPlantService.AddPowerPlant(createPowerPlantDto);
        }
        catch (DuplicateNameException e)
        {
            return BadRequest(e.Message);
        }
        return Ok();
    }
    
    /// <summary>
    /// Delete power plant
    /// </summary>
    /// <param name="id">id of the power plant to be deleted</param>
    /// <returns>Result of the process</returns>
    [HttpDelete]
    [Authorize(Roles = "user, admin")]
    public async Task<ActionResult> Delete(string id)
    {
        var result = await _powerPlantService.DeletePowerPlant(id);
        return result ? Ok() : BadRequest();
    }
}