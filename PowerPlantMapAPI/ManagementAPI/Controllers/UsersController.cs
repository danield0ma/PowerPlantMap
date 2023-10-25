using ManagementAPI.Data;
using ManagementAPI.Data.Dto;
using ManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagementAPI.Controllers;

[Route("API/[controller]")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet("GetByUserNameAsync")]
    [Authorize]
    public async Task<ActionResult<ApplicationUser?>> GetByUserNameAsync(string userName)
    {
        var user = await _userService.GetByUserNameAsync(userName);
        return user is null ? NoContent() : Ok(user);
    }
    
    [HttpGet("GetByEmailAsync")]
    [Authorize]
    public async Task<ActionResult<ApplicationUser?>> GetByEmailAsync(string email)
    {
        var user = await _userService.GetByUserNameAsync(email);
        return user is null ? NoContent() : Ok(user);
    }

    [HttpGet("GetAllAsync")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllAsync()
    {
        var users = await _userService.GetAll();
        return Ok(users);
    }
    
    [HttpPost("CreateUserAsync")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserDto createUserDto)
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

    [HttpDelete("DeleteUserAsync")]
    [Authorize]
    public async Task<IActionResult> DeleteUserAsync(string userName)
    {
        var result = await _userService.DeleteUserAsync(userName);
        return result ? Ok() : NoContent();
    }
}