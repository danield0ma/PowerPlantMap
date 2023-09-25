using PowerPlantMapAPI.Models.DTO;
using PowerPlantMapAPI.Models;
using PowerPlantMapAPI.Repositories;
using PowerPlantMapAPI.Services;
using System.Xml;
using System;
using Microsoft.Extensions.Configuration;

namespace PowerPlantMapAPI.Helpers
{
    public class PowerHelper : IPowerHelper
    {
        private readonly IDateService _dateService;
        private readonly IPowerRepository _repository;
        private readonly IConfiguration _configuration;

        public PowerHelper(IDateService dateService, IPowerRepository repository, IConfiguration configuration)
        {
            _dateService = dateService;
            _repository = repository;
            _configuration = configuration;
        }

        public async Task<string> APIquery(string documentType, DateTime start, DateTime end, string? inDomain = null, string? outDomain = null)
        {
            string periodStart = _dateService.EditTime(start);
            string periodEnd = _dateService.EditTime(end);

            string ProcessType = "A16";
            if (inDomain == null)
            {
                inDomain = "10YHU-MAVIR----U";
            }
            string BaseURL = "https://web-api.tp.entsoe.eu/api";
            string SecurityToken = GetAPIToken();

            if (SecurityToken != "")
            {
                string QueryString = "";
                if (documentType == "A11")
                {
                    QueryString = BaseURL + "?securityToken=" + SecurityToken +
                                       "&documentType=" + documentType +
                                       "&in_Domain=" + inDomain +
                                       "&out_Domain=" + outDomain +
                                       "&periodStart=" + periodStart +
                                       "&periodEnd=" + periodEnd;
                }
                else
                {
                    QueryString = BaseURL + "?securityToken=" + SecurityToken +
                                            "&documentType=" + documentType +
                                            "&processType=" + ProcessType +
                                            "&in_Domain=" + inDomain +
                                            "&periodStart=" + periodStart +
                                            "&periodEnd=" + periodEnd;
                }
                System.Diagnostics.Debug.WriteLine(QueryString);

                using var HttpClient = new HttpClient();
                var Response = await HttpClient.GetAsync(QueryString);
                string APIResponse = await Response.Content.ReadAsStringAsync();
                return APIResponse;
            }
            else
            {
                throw new FileNotFoundException("Hiányzik az API kulcsot tartalmazó file");
            }
        }

        public async Task<List<GeneratorPowerDTO>> GetGeneratorPower(string generator, DateTime start, DateTime end)
        {
            TimeZoneInfo cest = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            if (start.Kind != DateTimeKind.Utc)
            {
                start = TimeZoneInfo.ConvertTimeToUtc(start, TimeZoneInfo.Local);
                start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
                end = TimeZoneInfo.ConvertTimeToUtc(end, TimeZoneInfo.Local);
                end = DateTime.SpecifyKind(end, DateTimeKind.Utc);
            }

            List<PastActivityModel> PastActivity = await _repository.QueryPastActivity(generator, start, end);
            List<GeneratorPowerDTO> PastPowerOfGenerator = new List<GeneratorPowerDTO>();
            foreach (var Activity in PastActivity)
            {
                GeneratorPowerDTO GeneratorPower = new GeneratorPowerDTO();
                GeneratorPower.TimePoint = TimeZoneInfo.ConvertTimeFromUtc(Activity.PeriodStart, cest);
                GeneratorPower.Power = Activity.ActualPower;
                PastPowerOfGenerator.Add(GeneratorPower);
            }

            int NumberOfDataPoints = _dateService.CalculateTheNumberOfIntervals(start, end);

            for (int i = PastPowerOfGenerator.Count; i < NumberOfDataPoints; i++)
            {
                PastPowerOfGenerator.Add(new GeneratorPowerDTO());
            }
            return PastPowerOfGenerator;
        }

        public async Task<List<PowerStampDTO>> GetPowerStampsListOfPowerPlant(string Id, int NumberOfDataPoints, List<DateTime> TimeStamps)
        {
            List<PowerStampDTO> PowerStamps = new List<PowerStampDTO>();
            for (int i = 0; i < NumberOfDataPoints; i++)
            {
                PowerStampDTO PowerStamp = new PowerStampDTO();
                PowerStamp.Start = TimeStamps[0].AddMinutes(i * 15);
                PowerStamp.Power = 0;
                PowerStamps.Add(PowerStamp);
            }

            List<string> Generators = await _repository.QueryGeneratorsOfPowerPlant(Id);
            foreach (string Generator in Generators)
            {
                List<GeneratorPowerDTO> GeneratorPowers = await GetGeneratorPower(Generator, TimeStamps[0], TimeStamps[1]);
                foreach (GeneratorPowerDTO GeneratorPower in GeneratorPowers)
                {
                    List<PowerStampDTO> Matches = (List<PowerStampDTO>)PowerStamps.Where(x => x.Start == GeneratorPower.TimePoint).ToList();
                    foreach (PowerStampDTO Match in Matches)
                    {
                        Match.Power += GeneratorPower.Power;
                    }
                }
            }
            return PowerStamps;
        }

        private string GetAPIToken()
        {
            string securityToken = "";
            try
            {
                securityToken = _configuration["APIToken"];
            }
            catch (IOException Exception)
            {
                Console.WriteLine(Exception.Message);
            }
            return securityToken;
        }
    }
}
