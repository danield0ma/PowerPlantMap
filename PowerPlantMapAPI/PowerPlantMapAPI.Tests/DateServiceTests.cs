using FakeItEasy;
using PowerPlantMapAPI.Repositories;
using PowerPlantMapAPI.Services;
using Xunit;

namespace PowerPlantMapAPI.Tests
{
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
            DateService DateService = new DateService(_powerRepository);
            DateTime time = DateTime.Now;
            string start = DateService.EditTime(time);
            Assert.Equal(12, start.Length);
        }
    }
}