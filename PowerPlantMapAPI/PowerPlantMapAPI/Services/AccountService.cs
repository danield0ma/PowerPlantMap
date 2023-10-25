using PowerPlantMapAPI.Data;
using PowerPlantMapAPI.Data.Dto;

namespace PowerPlantMapAPI.Services;

public class AccountService : IAccountService
{
    private readonly IUserService _userService;
    
    public AccountService(IUserService userService)
    {
        _userService = userService;
    }
    
    public async Task<ApplicationUser?> LoginAsync(LoginDto loginDto)
    {
        var user = await _userService.GetByEmailAsync(loginDto.Email);

        if (user == null || user.PasswordHash != loginDto.Password)
        {
            return null;
        }

        return user;
    }
}