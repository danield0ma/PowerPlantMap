using PowerPlantMapAPI.Data;
using PowerPlantMapAPI.Data.Dto;

namespace PowerPlantMapAPI.Services;

public interface ITokenService
{
    TokenDto CreateToken(ApplicationUser user);
}