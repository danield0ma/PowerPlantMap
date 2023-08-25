﻿using PowerPlantMapAPI.Models.DTO;
using PowerPlantMapAPI.Models;
using PowerPlantMapAPI.Repositories;
using PowerPlantMapAPI.Services;
using System.Xml;

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

        public async Task<XmlDocument> APIquery(string DocumentType, string PeriodStart, string PeriodEnd)
        {
            string ProcessType = "A16";
            string InDomain = "10YHU-MAVIR----U";
            string BaseURL = "https://web-api.tp.entsoe.eu/api";
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

            if (SecurityToken != "")
            {
                string QueryString = BaseURL + "?securityToken=" + SecurityToken +
                                                "&documentType=" + DocumentType +
                                                "&processType=" + ProcessType +
                                                "&outBiddingZone_Domain=" + InDomain +
                                                "&periodStart=" + PeriodStart +
                                                "&periodEnd=" + PeriodEnd;

                var HttpClient = new HttpClient();
                var Response = await HttpClient.GetAsync(QueryString);
                string APIResponse = await Response.Content.ReadAsStringAsync();
                XmlDocument Document = new XmlDocument();
                Document.PreserveWhitespace = true;

                try
                {
                    Document.Load(new StringReader(APIResponse));
                }
                catch (Exception Exception)
                {
                    Console.WriteLine(Exception);
                }

                return Document;
            }
            else
            {
                throw new FileNotFoundException("Hiányzik az API kulcsot tartalmazó file");
            }
        }

        public async Task<List<GeneratorPowerDTO>> GetGeneratorPower(string generator, DateTime start, DateTime end)
        {
            List<PastActivityModel> PastActivity = await _repository.QueryPastActivity(generator, start, end);

            List<GeneratorPowerDTO> PastPowerOfGenerator = new List<GeneratorPowerDTO>();
            foreach (var Activity in PastActivity)
            {
                GeneratorPowerDTO GeneratorPower = new GeneratorPowerDTO();
                GeneratorPower.TimePoint = Activity.PeriodStart;
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
    }
}
