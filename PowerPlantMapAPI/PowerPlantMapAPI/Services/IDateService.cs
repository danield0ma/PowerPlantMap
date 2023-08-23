namespace PowerPlantMapAPI.Services
{
    public interface IDateService
    {
        Task<List<DateTime>> CheckDate(DateTime? date = null);
        string getTime(int diff);
        string EditTime(DateTime start);
        Task<List<DateTime>> GetInitDataTimeInterval();
        Task<List<DateTime>> GetLastDataTime();
        DateTime TransformTime(string time);
        int CalculateTheNumberOIntervals(DateTime Start, DateTime End);
    }
}
