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

/// <summary>
/// Manage the user accounts
/// </summary>
[EnableCors]
[Route("api/[controller]/[action]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    /// <summary>
    /// Instantiate the dependencies
    /// </summary>
    public AccountController(ITokenService tokenService,
        UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    /// <summary>
    /// Get all users
    /// </summary>
    /// <returns>List of all users</returns>
    [Authorize(Roles = "admin")]
    [HttpGet("")]
    public async Task<ActionResult<IEnumerable<ApplicationUser>>> Get()
    {
        var users = await _userManager.Users.ToListAsync();
        return Ok(users);
    }

    /// <summary>
    /// Get user by username
    /// </summary>
    /// <param name="userName">Username of the user</param>
    /// <returns>User with the given username</returns>
    [Authorize(Roles = "admin")]
    [HttpGet("")]
    public async Task<ActionResult<ApplicationUser?>> GetByUserName(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        return user is null ? NoContent() : Ok(user);
    }

    /// <summary>
    /// Get user by email
    /// </summary>
    /// <param name="email">Email of the user</param>
    /// <returns>User with the given email</returns>
    [Authorize(Roles = "admin")]
    [HttpGet("")]
    public async Task<ActionResult<ApplicationUser?>> GetByEmail(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user is null ? NoContent() : Ok(user);
    }
    
    /// <summary>
    /// Get the current user's profile data
    /// </summary>
    /// <returns>User's profile data</returns>
    [Authorize]
    [HttpGet("")]
    public async Task<ActionResult<UserProfileModelWrapper>> GetCurrentUserProfile()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userManager.FindByIdAsync(id);
        
        var userProfile = new UserProfileModel()
        {
            Id = id,
            Username = user.UserName,
            Email = user.Email,
            Role = User.FindFirst(ClaimTypes.Role)?.Value
        };

        return new UserProfileModelWrapper()
        {
            User = userProfile
        };
    }
    
    /// <summary>
    /// Register new user
    /// </summary>
    /// <param name="registrationModel">Registration data of the new user</param>
    /// <returns>Result of the registration</returns>
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

    /// <summary>
    /// Login the user
    /// </summary>
    /// <param name="loginModel">Username and password of the user</param>
    /// <returns>JWT Token</returns>
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

    /// <summary>
    /// Logout the user
    /// </summary>
    /// <returns>Result of the logout</returns>
    [Authorize]
    [HttpPost("")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }

    /// <summary>
    /// Change the username of the user
    /// </summary>
    /// <param name="currentUserName">The current username of the user</param>
    /// <param name="newUserName">The new username of the user</param>
    /// <returns>Result of the change</returns>
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

    /// <summary>
    /// Change password of the user
    /// </summary>
    /// <param name="userName">Username of the user</param>
    /// <param name="currentPassword">Old password of the user</param>
    /// <param name="newPassword">The new desired password of the user</param>
    /// <returns>Result of the process</returns>
    [Authorize]
    [HttpPut("")]
    public async Task<IActionResult> ChangePassword(string userName, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user is null) return BadRequest();
        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return result.Succeeded ? Ok() : NoContent();
    }

    /// <summary>
    /// Delete the specified user
    /// </summary>
    /// <param name="userName">Username of the user</param>
    /// <returns>Result of the process</returns>
    [Authorize(Roles = "admin")]
    [HttpDelete("")]
    public async Task<IActionResult> DeleteUser(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user is null) return BadRequest();
        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded ? Ok() : NoContent();
    }
}