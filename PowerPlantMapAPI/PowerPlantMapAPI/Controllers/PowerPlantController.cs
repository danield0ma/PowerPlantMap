using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PowerPlantMapAPI.Data.Dto;
using PowerPlantMapAPI.Services;

namespace PowerPlantMapAPI.Controllers;

[Route("API/[controller]")]
[ApiController]
[Authorize]
public class PowerPlantController : ControllerBase
{
    private readonly IPowerPlantService _powerPlantService;

    public PowerPlantController(IPowerPlantService powerPlantService)
    {
        _powerPlantService = powerPlantService;
    }
        
    [HttpGet("Get")]
    [AllowAnonymous]
    public async Task<ActionResult<PowerPlantDataDto?>> Get()
    {
        var powerPlants = await _powerPlantService.Get();
        return Ok(powerPlants);
    }
    
    [HttpGet("GetById")]
    [AllowAnonymous]
    public async Task<ActionResult<PowerPlantDataDto?>> GetById(string id)
    {
        var powerPlant = await _powerPlantService.GetById(id);
        return powerPlant is null ? NoContent() : Ok(powerPlant);
    }
    
    [HttpPost("Add")]
    [AllowAnonymous]
    public async Task<ActionResult> Add(PowerPlantDataDto powerPlantDataDto)
    {
        var result = await _powerPlantService.AddPowerPlant(powerPlantDataDto);
        return result ? Ok() : BadRequest();
    }
    
    [HttpDelete("Delete")]
    [AllowAnonymous]
    public async Task<ActionResult> Delete(string id)
    {
        var result = await _powerPlantService.DeletePowerPlant(id);
        return result ? Ok() : BadRequest();
    }
}