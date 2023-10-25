using ManagementAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace ManagementAPI.Repositories;

public class UserRepository: IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<ApplicationUser?> GetByUserNameAsync(string name)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.UserName == name);
    }
    
    public async Task<ApplicationUser?> GetByEmailAsync(string email)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<bool> AddUserAsync(ApplicationUser applicationUser)
    {
        if (await GetByUserNameAsync(applicationUser.UserName) is not null ||
            await GetByEmailAsync(applicationUser.Email) is not null)
        {
            return false;
        }
        await _context.Users.AddAsync(applicationUser);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> DeleteUserAsync(string userName)
    {
        var user = await GetByUserNameAsync(userName);
        if (user is null)
        {
            return false;
        }
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
}