using ManagementAPI.Data;
using ManagementAPI.Data.Dto;

namespace ManagementAPI.Services;

public interface IAccountService
{
    Task<ApplicationUser?> LoginAsync(LoginDto loginDto);
}