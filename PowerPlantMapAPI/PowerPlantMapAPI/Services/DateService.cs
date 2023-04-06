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

        public async Task<List<DateTime>> CheckDate(DateTime? date)
        {
            List<DateTime> TimeStamps = new List<DateTime>();
            if (date == null)
            {
                TimeStamps = await GetStartAndEnd(false);
            }
            else
            {
                DateTime Start = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, 0, 0, 0);
                date = date.Value.AddDays(1);
                DateTime End = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, 0, 0, 0);
                TimeStamps.Add(Start);
                TimeStamps.Add(End);
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

        public async Task<List<DateTime>> GetStartAndEnd(bool initData)
        {
            DateTime now = DateTime.Now;
            DateTime end = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            DateTime start;

            List<DateTime> LastData = await _repository.QueryLastDataTime();

            if (!initData)
            {
                //TODO túl régi adat esetén nincs elérhető adat kiírása...
                end = LastData[0];
                start = end.AddDays(-1).AddMinutes(-15);
            }
            else
            {
                if (now.Minute < 15)
                {
                    end = end.AddHours(now.Hour - 1);
                    end = end.AddMinutes(45);
                }
                else if (now.Minute < 30)
                {
                    end = end.AddHours((int)now.Hour);
                    end = end.AddMinutes(0);
                }
                else if (now.Minute < 45)
                {
                    end = end.AddHours(now.Hour);
                    end = end.AddMinutes(15);
                }
                else
                {
                    end = end.AddHours(now.Hour);
                    end = end.AddMinutes(30);
                }
                start = end.AddHours(-30);

                if (LastData[0] > start)
                {
                    start = LastData[0];
                }
            }

            return new List<DateTime> { start, end };
        }
    }
}
