using PowerPlantMapAPI.Repositories;

namespace PowerPlantMapAPI.Services
{
    public class DateService : IDateService
    {
        private readonly IPowerRepository _repository;

        public DateService(IPowerRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<DateTime>> HandleWhichDateFormatIsBeingUsed(DateTime? date = null, DateTime? startLocal = null, DateTime? endLocal = null)
        {
            List<DateTime> timeStampsUtc = new();
            //TimeZoneInfo cest = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

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
                //TODO túl régi adat esetén nincs elérhető adat kiírása...
                //DateTime lastDataTimeLocal = TimeZoneInfo.ConvertTimeFromUtc(lastDataTimeUtc[0], cest);
                //lastDataTimeLocal = DateTime.SpecifyKind(lastDataTimeLocal, DateTimeKind.Local);
                lastDataTimeUtc = DateTime.SpecifyKind(lastDataTimeUtc, DateTimeKind.Utc);
                startUtc = DateTime.SpecifyKind(startUtc, DateTimeKind.Utc);
                timeStampsUtc.Add(startUtc);
                timeStampsUtc.Add(lastDataTimeUtc);
            }

            return timeStampsUtc;
        }

        public string GetTime(int diff)
        {
            var now = Convert.ToString(DateTime.Now.Year);
            if (DateTime.Now.Month < 10) { now += "0"; }
            now += Convert.ToString(DateTime.Now.Month);
            if (DateTime.Now.Day < 10) { now += "0"; }
            now += Convert.ToString(DateTime.Now.Day);
            if (DateTime.Now.Hour - diff < 10) { now += "0"; }
            now += Convert.ToString(DateTime.Now.Hour - diff) + "00";
            return now;
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
            var now = DateTime.Now;
            DateTime endLocal = new(now.Year, now.Month, now.Day, now.Hour, 0, 0);

            if (now.Minute < 15)
            {
                endLocal = endLocal.AddHours(-1);
                endLocal = endLocal.AddMinutes(45);
            }
            else if (DateTime.Now.Minute < 30)
            {
                endLocal = endLocal.AddMinutes(0);
            }
            else if (DateTime.Now.Minute < 45)
            {
                endLocal = endLocal.AddMinutes(15);
            }
            else
            {
                endLocal = endLocal.AddMinutes(30);
            }
            
            endLocal = DateTime.SpecifyKind(endLocal, DateTimeKind.Local);
            var endUtc = TimeZoneInfo.ConvertTimeToUtc(endLocal, TimeZoneInfo.Local);
            
            var startUtc = endUtc.AddHours(-48);
            var lastDataUtc = await _repository.GetLastDataTime();
            if (lastDataUtc[0] > startUtc)
            {
                startUtc = lastDataUtc[0];
            }
            startUtc = DateTime.SpecifyKind(startUtc, DateTimeKind.Utc);

            return new List<DateTime> { startUtc, endUtc };
        }

        public DateTime TransformTime(string time)
        {
            var t = new DateTime(int.Parse(time[..4]),
                int.Parse(time.Substring(5, 2)), int.Parse(time.Substring(8, 2)),
                int.Parse(time.Substring(11, 2)), int.Parse(time.Substring(14, 2)), 00);

            return t;
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
}
