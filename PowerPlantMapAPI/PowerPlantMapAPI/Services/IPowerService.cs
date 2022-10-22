﻿using PowerPlantMapAPI.Models.DTO;

namespace PowerPlantMapAPI.Services
{
    public interface IPowerService
    {
        Task<CurrentLoadDTO> GetCurrentLoad(string periodStart, string periodEnd);
        Task<IEnumerable<CurrentLoadDTO>> GetLoadHistory(DateTime periodStart, DateTime periodEnd);
        string EditTime(DateTime start);
    }
}
