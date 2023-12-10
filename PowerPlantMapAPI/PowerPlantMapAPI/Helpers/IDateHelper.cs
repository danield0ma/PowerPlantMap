namespace PowerPlantMapAPI.Helpers;

public interface IDateHelper
{
    Task<List<DateTime>> HandleWhichDateFormatIsBeingUsed(DateTime? date = null, DateTime? start = null, DateTime? end = null);
    string ConvertTimeToApiStringFormat(DateTime time);
    Task<List<DateTime>> GetApiQueryTimeInterval();
    int CalculateTheNumberOfIntervals(DateTime start, DateTime end);
    List<DateTime> GetStartAndEndTimeOfDailyStatistics();
}
