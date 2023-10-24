using ManagementAPI.Data;
using ManagementAPI.Data.Dto;

namespace ManagementAPI.Services;

public interface ITokenService
{
    TokenDto CreateToken(ApplicationUser user);
}