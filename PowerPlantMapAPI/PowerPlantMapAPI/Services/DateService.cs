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

            if (date is DateTime)
            {
                date = DateTime.SpecifyKind((DateTime)date, DateTimeKind.Local);
                DateTime firstLocal = new(date.Value.Year, date.Value.Month, date.Value.Day, 0, 0, 0, DateTimeKind.Local);
                DateTime firstUtc = TimeZoneInfo.ConvertTimeToUtc(firstLocal, TimeZoneInfo.Local);
                firstUtc = DateTime.SpecifyKind(firstUtc, DateTimeKind.Utc);

                DateTime secondUtc = firstUtc.AddDays(1);
                secondUtc = DateTime.SpecifyKind(secondUtc, DateTimeKind.Utc);
                
                timeStampsUtc.Add(firstUtc);
                timeStampsUtc.Add(secondUtc);
            }
            else if (startLocal != null && endLocal != null)
            {
                startLocal = DateTime.SpecifyKind((DateTime)startLocal, DateTimeKind.Local);
                DateTime startUtc = TimeZoneInfo.ConvertTimeToUtc((DateTime)startLocal, TimeZoneInfo.Local);
                timeStampsUtc.Add(startUtc);
                
                endLocal = DateTime.SpecifyKind((DateTime)endLocal, DateTimeKind.Local);
                DateTime endUtc = TimeZoneInfo.ConvertTimeToUtc((DateTime)endLocal, TimeZoneInfo.Local);
                timeStampsUtc.Add(endUtc);
            }
            else
            {
                List<DateTime> lastDataTimeUtcArray = await _repository.QueryLastDataTime();
                DateTime lastDataTimeUtc = lastDataTimeUtcArray[0];
                DateTime startUtc = lastDataTimeUtc.AddDays(-1);
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

        public string getTime(int diff)
        {
            string now = Convert.ToString(DateTime.Now.Year);
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
            string startTime = Convert.ToString(start.Year);
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
            DateTime now = DateTime.Now;
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
            DateTime endUtc = TimeZoneInfo.ConvertTimeToUtc(endLocal, TimeZoneInfo.Local);
            
            DateTime startUtc = endUtc.AddHours(-48);
            List<DateTime> lastDataUtc = await _repository.QueryLastDataTime();
            if (lastDataUtc[0] > startUtc)
            {
                startUtc = lastDataUtc[0];
            }
            startUtc = DateTime.SpecifyKind(startUtc, DateTimeKind.Utc);

            return new List<DateTime> { startUtc, endUtc };
        }

        public DateTime TransformTime(string time)
        {
            DateTime t = new DateTime(Int32.Parse(time.Substring(0, 4)),
                Int32.Parse(time.Substring(5, 2)), Int32.Parse(time.Substring(8, 2)),
                Int32.Parse(time.Substring(11, 2)), Int32.Parse(time.Substring(14, 2)), 00);

            return t;
        }

        public int CalculateTheNumberOfIntervals(DateTime Start, DateTime End)
        {
            TimeSpan Period = End - Start;
            int NumberOfDataPoints = (int)Period.TotalMinutes / 15;
            return NumberOfDataPoints;
        }
    }
}
