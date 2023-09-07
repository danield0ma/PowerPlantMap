namespace PowerPlantMapAPI.Services
{
    public interface IDateService
    {
        Task<List<DateTime>> HandleWhichDateFormatIsBeingUsed(DateTime? date = null, DateTime? Start = null, DateTime? End = null);
        string getTime(int diff);
        string EditTime(DateTime start);
        Task<List<DateTime>> GetInitDataTimeInterval();
        //Task<List<DateTime>> GetLastDataTime();
        DateTime TransformTime(string time);
        int CalculateTheNumberOfIntervals(DateTime Start, DateTime End);
    }
}
