using ManagementAPI.Data;
using ManagementAPI.Data.Dto;
using ManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ManagementAPI.Controllers;

[EnableCors]
[Authorize]
[Route("API/[controller]/[action]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet]
    public async Task<ApplicationUser?> GetByUserNameAsync(string userName)
    {
        return await _userService.GetByUserNameAsync(userName);
    }
    
    [HttpGet]
    public async Task<ApplicationUser?> GetByEmailAsync(string email)
    {
        return await _userService.GetByUserNameAsync(email);
    }
    
    [Authorize(Roles = "admin")]
    [HttpGet]
    public async Task<IEnumerable<ApplicationUser>> GetAll()
    {
        return await _userService.GetAll();
    }

    [AllowAnonymous]
    [HttpGet]
    public string GetPassword()
    {
        return "Admin123";
    }
    
    [AllowAnonymous]
    [HttpGet]
    public string GetPasswordHash()
    {
        return "Asdadfafasasffasfassfkjhsfadmin123";
    }
    
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        if (await _userService.AddUserAsync(createUserDto))
        {
            return Ok();
        }
        else
        {
            return BadRequest();
        }
    }
}