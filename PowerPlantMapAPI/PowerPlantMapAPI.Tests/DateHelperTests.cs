using FakeItEasy;
using PowerPlantMapAPI.Helpers;
using PowerPlantMapAPI.Repositories;
using Xunit;

namespace PowerPlantMapAPI.Tests;

public class DateHelperTests
{
    private readonly IPowerDataRepository _powerDataRepository;

    public DateHelperTests()
    {
        _powerDataRepository = A.Fake<IPowerDataRepository>();
    }
    
    [Fact]
    public async void HandleWhichDateFormatIsBeingUsedWithEmptyData()
    {
        var fakeRepository = A.Fake<IPowerDataRepository>();
        var date = new DateTime(2023, 1, 1);
        var task = Task.FromResult(new List<DateTime> {date, date});
        A.CallTo(() => fakeRepository.GetLastDataTime()).Returns(task);
        
        var dateHelper = new DateHelper(fakeRepository);
        var timeStamps = await dateHelper.HandleWhichDateFormatIsBeingUsed();
        
        Assert.IsType<List<DateTime>>(timeStamps);
        Assert.Equal(2, timeStamps.Count);
        
        foreach (var timeStamp in timeStamps)
        {
            Assert.Equal("Utc", timeStamp.Kind.ToString());
        }
        
        var timeSpan = timeStamps[1] - timeStamps[0];
        Assert.Equal(24, timeSpan.TotalHours);
    }
    
    [Fact]
    public async void HandleWhichDateFormatIsBeingUsedWithDateGiven()
    {
        var dateHelper = new DateHelper(_powerDataRepository);
        var date = new DateTime(2023, 1, 1);
        var timeStamps = await dateHelper.HandleWhichDateFormatIsBeingUsed(date);
        
        Assert.IsType<List<DateTime>>(timeStamps);
        Assert.Equal(2, timeStamps.Count);
        
        foreach (var timeStamp in timeStamps)
        {
            Assert.Equal("Utc", timeStamp.Kind.ToString());
        }
        
        var timeSpan = timeStamps[1] - timeStamps[0];
        Assert.Equal(24, timeSpan.TotalHours);
    }
    
    [Fact]
    public async void HandleWhichDateFormatIsBeingUsedWithIntervalGiven()
    {
        var dateHelper = new DateHelper(_powerDataRepository);
        var start = new DateTime(2023, 1, 1);
        var end = new DateTime(2023, 1, 11);
        
        var timeStamps = await dateHelper.HandleWhichDateFormatIsBeingUsed(null, start, end);
        
        Assert.IsType<List<DateTime>>(timeStamps);
        Assert.Equal(2, timeStamps.Count);
        
        foreach (var timeStamp in timeStamps)
        {
            Assert.Equal("Utc", timeStamp.Kind.ToString());
        }
        
        var timeSpan = timeStamps[1] - timeStamps[0];
        Assert.Equal(240, timeSpan.TotalHours);
    }
    
    [Fact]
    public void ConvertTimeToApiStringFormatTypeAndLengthIsCorrect()
    {
        var dateHelper = new DateHelper(_powerDataRepository);
        var currentTimeUtc = DateTime.UtcNow;
        var start = dateHelper.ConvertTimeToApiStringFormat(currentTimeUtc);
        Assert.IsType<string>(start);
        Assert.Equal(12, start.Length);

        var date = new DateTime(2023, 1, 1, 0, 0, 0);
        start = dateHelper.ConvertTimeToApiStringFormat(date);
        Assert.IsType<string>(start);
        Assert.Equal(12, start.Length);
    }
}
