using ManagementAPI.Data.Dto;
using ManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ManagementAPI.Controllers;

[Authorize]
[EnableCors]
[Route("API/[controller]/[action]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ITokenService _tokenService;
    
    public AccountController(IAccountService accountService, ITokenService tokenService)
    {
        _accountService = accountService;
        _tokenService = tokenService;
    }
    
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<TokenDto>> LoginAsync([FromBody] LoginDto loginDto)
    {
        var user = await _accountService.LoginAsync(loginDto);
        if (user is null)
        {
            return Unauthorized();
        }

        var token = _tokenService.CreateToken(user);

        return Ok(token);
    }
}
