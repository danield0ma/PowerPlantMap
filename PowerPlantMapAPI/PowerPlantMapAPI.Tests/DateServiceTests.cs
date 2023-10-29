using FakeItEasy;
using PowerPlantMapAPI.Helpers;
using PowerPlantMapAPI.Repositories;
using Xunit;

namespace PowerPlantMapAPI.Tests;

public class DateServiceTests
{
    private readonly IPowerDataRepository _powerDataRepository;

    public DateServiceTests()
    {
        _powerDataRepository = A.Fake<IPowerDataRepository>();
    }

    [Fact]
    public void EditTimeIsLengthCorrect()
    {
        var dateHelper = new DateHelper(_powerDataRepository);
        var currentTimeUtc = DateTime.UtcNow;
        var start = dateHelper.EditTime(currentTimeUtc);
        Assert.Equal(12, start.Length);
    }
}
