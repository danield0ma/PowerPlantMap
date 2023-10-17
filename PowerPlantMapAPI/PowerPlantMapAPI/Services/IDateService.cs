namespace PowerPlantMapAPI.Services
{
    public interface IDateService
    {
        Task<List<DateTime>> HandleWhichDateFormatIsBeingUsed(DateTime? date = null, DateTime? start = null, DateTime? end = null);
        string EditTime(DateTime start);
        Task<List<DateTime>> GetInitDataTimeInterval();
        DateTime TransformTime(string time);
        int CalculateTheNumberOfIntervals(DateTime start, DateTime end);
    }
}