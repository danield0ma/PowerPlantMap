namespace PowerPlantMapAPI.Helpers;

public interface IDateHelper
{
    Task<List<DateTime>> HandleWhichDateFormatIsBeingUsed(DateTime? date = null, DateTime? start = null, DateTime? end = null);
    string EditTime(DateTime start);
    Task<List<DateTime>> GetInitDataTimeInterval();
    int CalculateTheNumberOfIntervals(DateTime start, DateTime end);
}
