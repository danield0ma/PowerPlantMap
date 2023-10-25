using PowerPlantMapAPI.Data;
using PowerPlantMapAPI.Data.Dto;
using PowerPlantMapAPI.Repositories;

namespace PowerPlantMapAPI.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<ApplicationUser?> GetByUserNameAsync(string userName)
    {
        return _userRepository.GetByUserNameAsync(userName);
    }

    public Task<ApplicationUser?> GetByEmailAsync(string email)
    {
        return _userRepository.GetByEmailAsync(email);
    }
    
    public async Task<IEnumerable<ApplicationUser>> GetAll()
    {
        return await _userRepository.GetAllAsync();
    }
    
    public async Task<bool> AddUserAsync(CreateUserDto newUser)
    {
        return await _userRepository.AddUserAsync(new ApplicationUser
        {
            UserName = newUser.UserName,
            Email = newUser.Email,
            PasswordHash = newUser.Password
        });
    }
    
    public async Task<bool> DeleteUserAsync(string userName)
    {
        return await _userRepository.DeleteUserAsync(userName);
    }
}