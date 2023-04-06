namespace PowerPlantMapAPI.Services
{
    public interface IDateService
    {
        Task<List<DateTime>> CheckDate(DateTime? date = null);
        string getTime(int diff);
        string EditTime(DateTime start);
        Task<List<DateTime>> GetStartAndEnd(bool initData);
        DateTime TransformTime(string time);
    }
}
