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
        
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PowerPlantDataDto?>> Get()
    {
        var powerPlants = await _powerPlantService.Get();
        return powerPlants is null ? NoContent() : Ok(powerPlants);
    }
}