using ManagementAPI.Data;
using ManagementAPI.Data.Dto;

namespace ManagementAPI.Services;

public interface IUserService
{
    Task<ApplicationUser?> GetByUserNameAsync(string userName);
    Task<ApplicationUser?> GetByEmailAsync(string email);
    Task<IEnumerable<ApplicationUser>> GetAll();
    Task<bool> AddUserAsync(CreateUserDto newUser);
    Task<bool> DeleteUserAsync(string userName);
}