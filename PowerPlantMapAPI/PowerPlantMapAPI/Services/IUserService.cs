using PowerPlantMapAPI.Data;
using PowerPlantMapAPI.Data.Dto;

namespace PowerPlantMapAPI.Services;

public interface IUserService
{
    Task<ApplicationUser?> GetByUserNameAsync(string userName);
    Task<ApplicationUser?> GetByEmailAsync(string email);
    Task<IEnumerable<ApplicationUser>> GetAll();
    Task<bool> AddUserAsync(CreateUserDto newUser);
    Task<bool> DeleteUserAsync(string userName);
}