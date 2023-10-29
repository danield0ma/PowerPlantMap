using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<ActionResult<PowerPlantDataDto?>> Get()
    {
        var powerPlants = await _powerPlantService.Get();
        return Ok(powerPlants);
    }
    
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PowerPlantDataDto?>> GetById(string id)
    {
        var powerPlant = await _powerPlantService.GetById(id);
        return powerPlant is null ? NoContent() : Ok(powerPlant);
    }
    
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> Add(PowerPlantDataDto powerPlantDataDto)
    {
        var result = await _powerPlantService.AddPowerPlant(powerPlantDataDto);
        return result ? Ok() : BadRequest();
    }
    
    [HttpPost]
    [AllowAnonymous]
    public ActionResult AddFullPowerPlant(CreatePowerPlantDto createPowerPlantDto)
    {
        return Ok("test");
        // var result = await _powerPlantService.AddPowerPlant(createPowerPlantDto);
        // return result ? Ok() : BadRequest();
    }
    
    [HttpDelete]
    [AllowAnonymous]
    public async Task<ActionResult> Delete(string id)
    {
        var result = await _powerPlantService.DeletePowerPlant(id);
        return result ? Ok() : BadRequest();
    }
}