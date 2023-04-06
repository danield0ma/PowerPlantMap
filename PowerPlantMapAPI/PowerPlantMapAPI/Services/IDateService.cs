namespace PowerPlantMapAPI.Services
{
    public interface IDateService
    {
        Task<List<DateTime>> CheckDate(DateTime? date);
        string getTime(int diff);
        string EditTime(DateTime start);
        Task<List<DateTime>> GetStartAndEnd(bool initData);
    }
}
