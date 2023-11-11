﻿using PowerPlantMapAPI.Data.Dto;
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
    
    [Authorize(Roles = "admin")]
    [HttpGet("GetAllAsync")]
    public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        return Ok(users);
    }
    
    [Authorize(Roles = "admin")]
    [HttpGet("GetByUserNameAsync")]
    public async Task<ActionResult<ApplicationUser?>> GetByUserNameAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        return user is null ? NoContent() : Ok(user);
    }
    
    [Authorize(Roles = "admin")]
    [HttpGet("GetByEmailAsync")]
    public async Task<ActionResult<ApplicationUser?>> GetByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user is null ? NoContent() : Ok(user);
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

        if (registerDto.Password is null) return BadRequest(new { message = "Password can't be null" });
        var result = await _userManager.CreateAsync(user, registerDto.Password);
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
    [HttpPost("LoginAsync")]
    public async Task<ActionResult<TokenDto>> LoginAsync([FromBody] LoginDto loginDto)
    {
        var user = await _userManager.FindByNameAsync(loginDto.UserName);

        if (user is null || !await _userManager.CheckPasswordAsync(user, loginDto.Password)) return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user);
        
        var token = _tokenService.CreateToken(user, roles);
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
    [HttpPut("ChangeUserNameAsync")]
    public async Task<IActionResult> ChangeUserNameAsync(string currentUserName, string newUserName)
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
    [HttpPut("ChangePasswordAsync")]
    public async Task<IActionResult> ChangePasswordAsync(string userName, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user is null) return BadRequest();
        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return result.Succeeded ? Ok() : NoContent();
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
