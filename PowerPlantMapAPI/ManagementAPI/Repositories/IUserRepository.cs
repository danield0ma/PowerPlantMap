using ManagementAPI.Data;

namespace ManagementAPI.Repositories;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByUserNameAsync(string name);
    Task<ApplicationUser?> GetByEmailAsync(string email);
    Task<IEnumerable<ApplicationUser>> GetAllAsync();
    Task<bool> AddUserAsync(ApplicationUser applicationUser);
    Task<bool> DeleteUserAsync(string userName);
}