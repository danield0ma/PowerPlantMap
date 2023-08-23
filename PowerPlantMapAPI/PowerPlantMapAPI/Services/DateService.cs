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

        public async Task<List<DateTime>> HandleWhichDateFormatIsBeingUsed(DateTime? Date = null, DateTime? Start = null, DateTime? End = null)
        {
            List<DateTime> TimeStamps = new List<DateTime>();

            if (Date != null)
            {
                TimeStamps.Add(new DateTime(Date.Value.Year, Date.Value.Month, Date.Value.Day, 0, 0, 0));
                Date = Date.Value.AddDays(1);
                TimeStamps.Add(new DateTime(Date.Value.Year, Date.Value.Month, Date.Value.Day, 0, 0, 0));
            }
            else if (Start != null && End != null)
            {
                TimeStamps.Add((DateTime)Start);
                TimeStamps.Add((DateTime)End);
            }
            else
            {
                TimeStamps = await GetLastDataTime();
            }

            return TimeStamps;
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
            string StartTime = Convert.ToString(start.Year);
            if (start.Month < 10) { StartTime += "0"; }
            StartTime += Convert.ToString(start.Month);
            if (start.Day < 10) { StartTime += "0"; }
            StartTime += Convert.ToString(start.Day);
            if (start.Hour < 10) { StartTime += "0"; }
            StartTime += Convert.ToString(start.Hour);
            if (start.Minute < 10) { StartTime += "0"; }
            StartTime += Convert.ToString(start.Minute);
            return StartTime;
        }

        public async Task<List<DateTime>> GetInitDataTimeInterval()
        {
            DateTime end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

            if (DateTime.Now.Minute < 15)
            {
                end = end.AddHours(-1);
                end = end.AddMinutes(45);
            }
            else if (DateTime.Now.Minute < 30)
            {
                end = end.AddMinutes(0);
            }
            else if (DateTime.Now.Minute < 45)
            {
                end = end.AddMinutes(15);
            }
            else
            {
                end = end.AddMinutes(30);
            }
            //start = end.AddHours(-30);

            List<DateTime> LastData = await _repository.QueryLastDataTime();
            if (LastData[0] > start)
            {
                start = LastData[0];
            }

            return new List<DateTime> { start, end };
        }

        public async Task<List<DateTime>> GetLastDataTime()
        {
            List<DateTime> LastDataTime = await _repository.QueryLastDataTime();

            //TODO túl régi adat esetén nincs elérhető adat kiírása...
            DateTime End = LastDataTime[0];
            DateTime Start = End.AddDays(-1)/*.AddMinutes(-15)*/;

            return new List<DateTime> { Start, End };
        }

        public DateTime TransformTime(string time)
        {
            DateTime t = new DateTime(Int32.Parse(time.Substring(0, 4)),
                Int32.Parse(time.Substring(5, 2)), Int32.Parse(time.Substring(8, 2)),
                Int32.Parse(time.Substring(11, 2)), Int32.Parse(time.Substring(14, 2)), 00);

            return t;
        }

        public int CalculateTheNumberOIntervals(DateTime Start, DateTime End)
        {
            TimeSpan Period = End - Start;
            int NumberOfDataPoints = (int)Period.TotalMinutes / 15;
            return NumberOfDataPoints;
        }
    }
}
