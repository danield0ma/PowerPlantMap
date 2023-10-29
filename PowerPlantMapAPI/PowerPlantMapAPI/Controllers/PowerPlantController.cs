using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PowerPlantMapAPI.Data;
using PowerPlantMapAPI.Data.Dto;
using PowerPlantMapAPI.Services;

namespace PowerPlantMapAPI.Controllers;

[Route("API/[controller]/[action]")]
[ApiController]
[Authorize]
public class PowerPlantController : ControllerBase
{
    private readonly IPowerPlantService _powerPlantService;

    public PowerPlantController(IPowerPlantService powerPlantService)
    {
        _powerPlantService = powerPlantService;
    }
        
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PowerPlantModel?>?>> Get()
    {
        var powerPlants = await _powerPlantService.Get();
        return Ok(powerPlants);
    }
    
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PowerPlantModel?>> GetById(string id)
    {
        var powerPlant = await _powerPlantService.GetById(id);
        return powerPlant is null ? NoContent() : Ok(powerPlant);
    }
    
    [HttpPost]
    [AllowAnonymous]
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
    
    [HttpDelete]
    [AllowAnonymous]
    public async Task<ActionResult> Delete(string id)
    {
        var result = await _powerPlantService.DeletePowerPlant(id);
        return result ? Ok() : BadRequest();
    }
}