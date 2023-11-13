using System.Security.Claims;
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
[Route("api/[controller]/[action]")]
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

    [Authorize(Roles = "admin")]
    [HttpGet("")]
    public async Task<ActionResult<IEnumerable<ApplicationUser>>> Get()
    {
        var users = await _userManager.Users.ToListAsync();
        return Ok(users);
    }

    [Authorize(Roles = "admin")]
    [HttpGet("")]
    public async Task<ActionResult<ApplicationUser?>> GetByUserName(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        return user is null ? NoContent() : Ok(user);
    }

    [Authorize(Roles = "admin")]
    [HttpGet("")]
    public async Task<ActionResult<ApplicationUser?>> GetByEmail(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user is null ? NoContent() : Ok(user);
    }
    
    [Authorize]
    [HttpGet("")]
    public async Task<ActionResult<UserProfileModel>> GetCurrentUserProfile()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userManager.FindByIdAsync(id);
        
        return new UserProfileModel()
        {
            Id = id,
            UserName = user.UserName,
            Email = user.Email,
            Role = User.FindFirst(ClaimTypes.Role)?.Value
        };
    }

    [AllowAnonymous]
    [HttpPost("")]
    public async Task<IActionResult> Register([FromBody] RegistrationModel registrationModel)
    {
        if (!ModelState.IsValid) return BadRequest(new { message = "Invalid registration data" });

        var user = new ApplicationUser
        {
            UserName = registrationModel.UserName,
            Email = registrationModel.Email
        };

        if (registrationModel.Password is null) return BadRequest(new { message = "Password can't be null" });
        var result = await _userManager.CreateAsync(user, registrationModel.Password);
        // var queriedUser = await _userManager.FindByNameAsync(registerDto.UserName!);

        if (result.Succeeded)
        {
            // var token = await _userManager.GenerateEmailConfirmationTokenAsync(queriedUser!);
            // send email with token
            return Ok(new { message = "User registered successfully" });
        }

        return BadRequest(new { message = "User registration failed", errors = result.Errors });
    }

    // [AllowAnonymous]
    // [HttpPost("ConfirmEmailAsync")]
    // public async Task<IActionResult> ConfirmEmailAsync(string userName, string token)
    // {
    //     var user = await _userManager.FindByNameAsync(userName);
    //     if (user is null) return BadRequest();
    //     var result = await _userManager.ConfirmEmailAsync(user, token);
    //     return result.Succeeded ? Ok() : NoContent();
    // }

    [AllowAnonymous]
    [HttpPost("")]
    public async Task<ActionResult<TokenDto>> Login([FromBody] LoginModel loginModel)
    {
        var user = await _userManager.FindByNameAsync(loginModel.UserName);

        if (user is null || !await _userManager.CheckPasswordAsync(user, loginModel.Password)) return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user);

        var token = _tokenService.CreateToken(user, roles);
        return Ok(token);
    }

    [Authorize]
    [HttpPost("")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }

    [Authorize]
    [HttpPut("")]
    public async Task<IActionResult> ChangeUserName(string currentUserName, string newUserName)
    {
        var user = await _userManager.FindByNameAsync(currentUserName);
        if (user is null) return BadRequest();
        var result = await _userManager.SetUserNameAsync(user, newUserName);
        return result.Succeeded ? Ok() : NoContent();
    }

    // [AllowAnonymous]
    // [HttpPut("InitiateChangeEmailAsync")]
    // public async Task<IActionResult> InitiateChangeEmailAsync(string userName, string newEmail)
    // {
    //     var user = await _userManager.FindByNameAsync(userName);
    //     if (user is null) return BadRequest();
    //     // var result = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
    //     //Send email with token
    //     return Ok();
    // }

    // [AllowAnonymous]
    // [HttpPut("ChangeEmailAsync")]
    // public async Task<IActionResult> ChangeEmailAsync(string userName, string newEmail, string token)
    // {
    //     var user = await _userManager.FindByNameAsync(userName);
    //     if (user is null) return BadRequest();
    //     var result = await _userManager.ChangeEmailAsync(user, newEmail, token);
    //     return result.Succeeded ? Ok() : NoContent();
    // }

    [Authorize]
    [HttpPut("")]
    public async Task<IActionResult> ChangePassword(string userName, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user is null) return BadRequest();
        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return result.Succeeded ? Ok() : NoContent();
    }

    [Authorize]
    [HttpDelete("")]
    public async Task<IActionResult> DeleteUser(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user is null) return BadRequest();
        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded ? Ok() : NoContent();
    }
}