using PowerPlantMapAPI.Data.Dto;
using PowerPlantMapAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PowerPlantMapAPI.Data;

namespace PowerPlantMapAPI.Controllers;

[EnableCors]
[Route("API/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    
    public AccountController(ITokenService tokenService,
        UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _signInManager = signInManager;
    }
    
    [Authorize]
    [HttpGet("GetByUserNameAsync")]
    public async Task<ActionResult<ApplicationUser?>> GetByUserNameAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        return user is null ? NoContent() : Ok(user);
    }
    
    [Authorize]
    [HttpGet("GetByEmailAsync")]
    public async Task<ActionResult<ApplicationUser?>> GetByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user is null ? NoContent() : Ok(user);
    }
    
    [Authorize]
    [HttpGet("GetAllAsync")]
    public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        return Ok(users);
    }
    
    [AllowAnonymous]
    [HttpPost("RegisterAsync")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid) return BadRequest(new { message = "Invalid registration data" });
        
        var user = new ApplicationUser
        {
            UserName = registerDto.UserName,
            Email = registerDto.Email
        };

        if (registerDto.Password != null)
        {
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                return Ok(new { message = "User registered successfully" });
            }
        
            return BadRequest(new { message = "User registration failed", errors = result.Errors });
        }
        return BadRequest(new { message = "Password can't be null" });
    }
    
    [AllowAnonymous]
    [HttpPost("LoginAsync")]
    public async Task<ActionResult<TokenDto>> LoginAsync([FromBody] LoginDto loginDto)
    {
        var user = await _userManager.FindByNameAsync(loginDto.UserName);

        if (user is null || !await _userManager.CheckPasswordAsync(user, loginDto.Password)) return Unauthorized();
        
        var token = _tokenService.CreateToken(user);
        return Ok(token);
    }
    
    [Authorize]
    [HttpPost("LogoutAsync")]
    public async Task<IActionResult> LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }
    
    [Authorize]
    [HttpDelete("DeleteUserAsync")]
    public async Task<IActionResult> DeleteUserAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user is null) return BadRequest();
        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded ? Ok() : NoContent();
    }
}
