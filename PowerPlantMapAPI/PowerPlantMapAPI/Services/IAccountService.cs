using PowerPlantMapAPI.Data;
using PowerPlantMapAPI.Data.Dto;

namespace PowerPlantMapAPI.Services;

public interface IAccountService
{
    Task<ApplicationUser?> LoginAsync(LoginDto loginDto);
}