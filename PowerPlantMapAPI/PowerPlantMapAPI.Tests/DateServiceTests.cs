using FakeItEasy;
using PowerPlantMapAPI.Helpers;
using PowerPlantMapAPI.Repositories;
using Xunit;

namespace PowerPlantMapAPI.Tests;

public class DateServiceTests
{
    private readonly IPowerRepository _powerRepository;

    public DateServiceTests()
    {
        _powerRepository = A.Fake<IPowerRepository>();
    }

    [Fact]
    public void EditTimeIsLengthCorrect()
    {
        var dateHelper = new DateHelper(_powerRepository);
        var currentTimeUtc = DateTime.UtcNow;
        var start = dateHelper.EditTime(currentTimeUtc);
        Assert.Equal(12, start.Length);
    }
}
