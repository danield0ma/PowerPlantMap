﻿using PowerPlantMapAPI.Models.DTO;
using PowerPlantMapAPI.Models;
using PowerPlantMapAPI.Repositories;
using PowerPlantMapAPI.Services;
using System.Xml;
using System;

namespace PowerPlantMapAPI.Helpers
{
    public class PowerHelper : IPowerHelper
    {
        private readonly IDateService _dateService;
        private readonly IPowerRepository _repository;

        public PowerHelper(IDateService dateService, IPowerRepository repository)
        {
            _dateService = dateService;
            _repository = repository;
        }

        public async Task<string> APIquery(string DocumentType, string PeriodStart, string PeriodEnd, string? InDomain = null, string? OutDomain = null)
        {
            string ProcessType = "A16";
            if (InDomain == null)
            {
                InDomain = "10YHU-MAVIR----U";
            }
            string BaseURL = "https://web-api.tp.entsoe.eu/api";
            string SecurityToken = GetAPIToken();

            if (SecurityToken != "")
            {
                string QueryString = "";
                if (DocumentType == "A11")
                {
                    QueryString = BaseURL + "?securityToken=" + SecurityToken +
                                       "&documentType=" + DocumentType +
                                       "&in_Domain=" + InDomain +
                                       "&out_Domain=" + OutDomain +
                                       "&periodStart=" + PeriodStart +
                                       "&periodEnd=" + PeriodEnd;
                }
                else
                {
                    QueryString = BaseURL + "?securityToken=" + SecurityToken +
                                            "&documentType=" + DocumentType +
                                            "&processType=" + ProcessType +
                                            "&in_Domain=" + InDomain +
                                            "&periodStart=" + PeriodStart +
                                            "&periodEnd=" + PeriodEnd;
                }
                Console.WriteLine(QueryString);

                var HttpClient = new HttpClient();
                var Response = await HttpClient.GetAsync(QueryString);
                string APIResponse = await Response.Content.ReadAsStringAsync();
                return APIResponse;

                //XmlDocument Document = new XmlDocument();
                //Document.PreserveWhitespace = true;

                //try
                //{
                //    Document.Load(new StringReader(APIResponse));
                //    Document.Save("XMLData/" + QueryString + ".xml");
                //}
                //catch (Exception Exception)
                //{
                //    Console.WriteLine(Exception);
                //}

                //return Document;
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
            string SecurityToken = "";
            try
            {
                using (var StreamReader = new StreamReader("key.txt"))
                {
                    SecurityToken = StreamReader.ReadToEnd();
                }
            }
            catch (IOException Exception)
            {
                Console.WriteLine(Exception.Message);
            }
            return SecurityToken;
        }
    }
}
