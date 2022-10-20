using PowerPlantMapAPI.Models.DTO;

namespace PowerPlantMapAPI.Services
{
    public interface IPowerService
    {
        Task<CurrentLoadDTO> APIquery(string periodStart, string periodEnd);
    }
}
