using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PowerPlantMapAPI.Data;
using PowerPlantMapAPI.Data.Dto;
using PowerPlantMapAPI.Services;

namespace PowerPlantMapAPI.Controllers;

[Route("api/[controller]/[action]")]
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
    
    // [HttpPut]
    // [AllowAnonymous]
    // public async Task<ActionResult> UpdatePowerPlant(RegisterDto updateUserDto)
    // {
    //     
    //     
    //     try
    //     {
    //         await _powerPlantService.UpdatePowerPlant(updatePowerPlantDto);
    //         return Ok();
    //     }
    //     catch (ArgumentException e)
    //     {
    //         Console.WriteLine(e.Message);
    //         return BadRequest(e.Message);
    //     }
    // }
    
    [HttpDelete]
    [Authorize(Roles = "user, admin")]
    public async Task<ActionResult> Delete(string id)
    {
        var result = await _powerPlantService.DeletePowerPlant(id);
        return result ? Ok() : BadRequest();
    }
}