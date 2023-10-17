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

        public async Task<string> APIquery(string documentType, DateTime startUtc, DateTime endUtc, string? inDomain = null, string? outDomain = null)
        {
            string periodStartUtc = _dateService.EditTime(startUtc);
            string periodEndUtc = _dateService.EditTime(endUtc);

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
                                       "&periodStart=" + periodStartUtc +
                                       "&periodEnd=" + periodEndUtc;
                }
                else
                {
                    QueryString = BaseURL + "?securityToken=" + SecurityToken +
                                            "&documentType=" + documentType +
                                            "&processType=" + ProcessType +
                                            "&in_Domain=" + inDomain +
                                            "&periodStart=" + periodStartUtc +
                                            "&periodEnd=" + periodEndUtc;
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

        public async Task<List<GeneratorPowerDTO>> GetGeneratorPower(string generator, DateTime startUtc, DateTime endUtc)
        {
            //if (startUtc.Kind != DateTimeKind.Utc)
            //{
            //    startUtc = TimeZoneInfo.ConvertTimeToUtc(startUtc, TimeZoneInfo.Local);
            //    startUtc = DateTime.SpecifyKind(startUtc, DateTimeKind.Utc);
            //    endUtc = TimeZoneInfo.ConvertTimeToUtc(endUtc, TimeZoneInfo.Local);
            //    endUtc = DateTime.SpecifyKind(endUtc, DateTimeKind.Utc);
            //}

            List<PastActivityModel> pastActivity = await _repository.GetPastActivity(generator, startUtc, endUtc);
            List<GeneratorPowerDTO> pastPowerOfGenerator = new();
            foreach (var activity in pastActivity)
            {
                GeneratorPowerDTO generatorPower = new()
                {
                    TimePoint = TimeZoneInfo.ConvertTimeFromUtc(activity.PeriodStart, TimeZoneInfo.Local),
                    Power = activity.ActualPower
                };
                pastPowerOfGenerator.Add(generatorPower);
            }

            int numberOfDataPoints = _dateService.CalculateTheNumberOfIntervals(startUtc, endUtc);

            for (int i = pastPowerOfGenerator.Count; i < numberOfDataPoints; i++)
            {
                pastPowerOfGenerator.Add(new GeneratorPowerDTO());
            }
            return pastPowerOfGenerator;
        }

        public async Task<List<PowerStampDTO>> GetPowerStampsListOfPowerPlant(string id, int numberOfDataPoints, List<DateTime> timeStampsUtc)
        {
            List<PowerStampDTO> powerStamps = new();
            for (int i = 0; i < numberOfDataPoints; i++)
            {
                PowerStampDTO powerStamp = new();
                powerStamp.Start = TimeZoneInfo.ConvertTimeFromUtc(timeStampsUtc[0].AddMinutes(i * 15), TimeZoneInfo.Local);
                powerStamp.Power = 0;
                powerStamps.Add(powerStamp);
            }

            List<string> generators = await _repository.GetGeneratorsOfPowerPlant(id);
            foreach (string generator in generators)
            {
                List<GeneratorPowerDTO> generatorPowers = await GetGeneratorPower(generator, timeStampsUtc[0], timeStampsUtc[1]);
                foreach (GeneratorPowerDTO generatorPower in generatorPowers)
                {
                    DateTime generatorPowerTimePointLocal = generatorPower.TimePoint;
                    List<PowerStampDTO> matches = powerStamps.Where(x => x.Start == generatorPowerTimePointLocal).ToList();
                    foreach (PowerStampDTO match in matches)
                    {
                        match.Power += generatorPower.Power;
                    }
                }
            }
            return powerStamps;
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
