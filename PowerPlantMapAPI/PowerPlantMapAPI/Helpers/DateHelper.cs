using PowerPlantMapAPI.Repositories;
using PowerPlantMapAPI.Services;

namespace PowerPlantMapAPI.Helpers;

public class DateHelper : IDateHelper
{
    private readonly IPowerRepository _repository;

    public DateHelper(IPowerRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<DateTime>> HandleWhichDateFormatIsBeingUsed(DateTime? date = null, DateTime? startLocal = null, DateTime? endLocal = null)
    {
        List<DateTime> timeStampsUtc = new();

        if (date is not null)
        {
            date = DateTime.SpecifyKind((DateTime)date, DateTimeKind.Local);
            DateTime firstLocal = new(date.Value.Year, date.Value.Month, date.Value.Day, 0, 0, 0, DateTimeKind.Local);
            var firstUtc = TimeZoneInfo.ConvertTimeToUtc(firstLocal, TimeZoneInfo.Local);
            firstUtc = DateTime.SpecifyKind(firstUtc, DateTimeKind.Utc);

            var secondUtc = firstUtc.AddDays(1);
            secondUtc = DateTime.SpecifyKind(secondUtc, DateTimeKind.Utc);
            
            timeStampsUtc.Add(firstUtc);
            timeStampsUtc.Add(secondUtc);
        }
        else if (startLocal != null && endLocal != null)
        {
            startLocal = DateTime.SpecifyKind((DateTime)startLocal, DateTimeKind.Local);
            var startUtc = TimeZoneInfo.ConvertTimeToUtc((DateTime)startLocal, TimeZoneInfo.Local);
            timeStampsUtc.Add(startUtc);
            
            endLocal = DateTime.SpecifyKind((DateTime)endLocal, DateTimeKind.Local);
            var endUtc = TimeZoneInfo.ConvertTimeToUtc((DateTime)endLocal, TimeZoneInfo.Local);
            timeStampsUtc.Add(endUtc);
        }
        else
        {
            var lastDataTimeUtcArray = await _repository.GetLastDataTime();
            var lastDataTimeUtc = lastDataTimeUtcArray[0];
            var startUtc = lastDataTimeUtc.AddDays(-1);
            lastDataTimeUtc = DateTime.SpecifyKind(lastDataTimeUtc, DateTimeKind.Utc);
            startUtc = DateTime.SpecifyKind(startUtc, DateTimeKind.Utc);
            timeStampsUtc.Add(startUtc);
            timeStampsUtc.Add(lastDataTimeUtc);
        }

        return timeStampsUtc;
    }

    public string EditTime(DateTime start)
    {
        var startTime = Convert.ToString(start.Year);
        if (start.Month < 10) { startTime += "0"; }
        startTime += Convert.ToString(start.Month);
        if (start.Day < 10) { startTime += "0"; }
        startTime += Convert.ToString(start.Day);
        if (start.Hour < 10) { startTime += "0"; }
        startTime += Convert.ToString(start.Hour);
        if (start.Minute < 10) { startTime += "0"; }
        startTime += Convert.ToString(start.Minute);
        return startTime;
    }

    public async Task<List<DateTime>> GetInitDataTimeInterval()
    {
        var utcNow = DateTime.UtcNow;
        DateTime endUtc = new(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, 0, 0);

        switch (utcNow.Minute)
        {
            case < 15:
                endUtc = endUtc.AddHours(-1);
                endUtc = endUtc.AddMinutes(45 - endUtc.Minute);
                break;
            case < 30:
                endUtc = endUtc.AddMinutes(endUtc.Minute * -1);
                break;
            case < 45:
                endUtc = endUtc.AddMinutes((endUtc.Minute - 15) * -1);
                break;
            default:
                endUtc = endUtc.AddMinutes((endUtc.Minute - 30) * -1);
                break;
        }
        
        endUtc = DateTime.SpecifyKind(endUtc, DateTimeKind.Utc);
        var startUtc = endUtc.AddHours(-48);
        var lastDataUtc = await _repository.GetLastDataTime();
        if (lastDataUtc[0] > startUtc)
        {
            startUtc = lastDataUtc[0];
        }
        startUtc = DateTime.SpecifyKind(startUtc, DateTimeKind.Utc);

        return new List<DateTime> { startUtc, endUtc };
    }

    public int CalculateTheNumberOfIntervals(DateTime start, DateTime end)
    {
        var period = end - start;
        var numberOfDataPoints = (int)period.TotalMinutes / 15;
        return numberOfDataPoints;
    }
    
    public List<DateTime> GetStartAndEndTimeOfDailyStatistics()
    {
        var yesterday = DateTime.Today.AddDays(-1);
        var start = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 0, 0, 0);
        var today = DateTime.Today;
        var end = new DateTime(today.Year, today.Month, today.Day, 0, 0, 0);
        return new List<DateTime> { start, end };
    }
}