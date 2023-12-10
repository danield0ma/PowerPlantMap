using PowerPlantMapAPI.Data;
using PowerPlantMapAPI.Data.Dto;
using PowerPlantMapAPI.Repositories;

namespace PowerPlantMapAPI.Helpers;

public class PowerDataHelper : IPowerDataHelper
{
    private readonly IDateHelper _dateHelper;
    private readonly IPowerDataRepository _dataRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PowerDataHelper> _logger;

    public PowerDataHelper(IDateHelper dateHelper, IPowerDataRepository dataRepository, IConfiguration configuration, ILogger<PowerDataHelper> logger)
    {
        _dateHelper = dateHelper;
        _dataRepository = dataRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> MakeApiQuery(string documentType, DateTime startUtc, DateTime endUtc, string? inDomain = null, string? outDomain = null)
    {
        var periodStartUtc = _dateHelper.ConvertTimeToApiStringFormat(startUtc);
        var periodEndUtc = _dateHelper.ConvertTimeToApiStringFormat(endUtc);

        const string processType = "A16";
        inDomain ??= "10YHU-MAVIR----U";
        const string baseUrl = "https://web-api.tp.entsoe.eu/api";
        var securityToken = GetApiToken();

        if (string.IsNullOrEmpty(securityToken))
        {
            throw new FileNotFoundException("Hiányzik az API kulcsot tartalmazó file");
        }
        
        var queryString = 
            documentType == "A11"
                ? baseUrl + "?securityToken=" + securityToken +
                  "&documentType=" + documentType +
                  "&in_Domain=" + inDomain +
                  "&out_Domain=" + outDomain +
                  "&periodStart=" + periodStartUtc +
                  "&periodEnd=" + periodEndUtc
                : baseUrl + "?securityToken=" + securityToken +
                  "&documentType=" + documentType +
                  "&processType=" + processType +
                  "&in_Domain=" + inDomain +
                  "&periodStart=" + periodStartUtc +
                  "&periodEnd=" + periodEndUtc;
        
        _logger.LogInformation("API query: {QueryString}", queryString);

        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(queryString);
        var apiResponse = await response.Content.ReadAsStringAsync();
        
        _logger.LogInformation("Got API response");
        return apiResponse;
    }

    public async Task<List<GeneratorPowerDto>?> GetGeneratorPower(string? generator, DateTime startUtc, DateTime endUtc)
    {
        var pastActivity = await _dataRepository.GetPastActivity(generator, startUtc, endUtc);
        var pastPowerOfGenerator = pastActivity.Select
            (activity => new GeneratorPowerDto() { 
                TimePoint = TimeZoneInfo.ConvertTimeFromUtc(activity.PeriodStart, TimeZoneInfo.Local),
                Power = activity.ActualPower
            }).ToList();

        var numberOfDataPoints = _dateHelper.CalculateTheNumberOfIntervals(startUtc, endUtc);

        for (var i = pastPowerOfGenerator.Count; i < numberOfDataPoints; i++)
        {
            pastPowerOfGenerator.Add(new GeneratorPowerDto());
        }
        return pastPowerOfGenerator;
    }

    public async Task<List<PowerOfPowerPlantModel>> GetPowerStampsListOfPowerPlant(string id, int numberOfDataPoints, List<DateTime> timeStamps)
    {
        List<PowerOfPowerPlantModel> powerStamps = new();
        for (var i = 0; i < numberOfDataPoints; i++)
        {
            PowerOfPowerPlantModel powerStamp = new()
            {
                Start = TimeZoneInfo.ConvertTimeFromUtc(timeStamps[0].AddMinutes(i * 15), TimeZoneInfo.Local),
                Power = 0
            };
            powerStamps.Add(powerStamp);
        }

        var generators = await _dataRepository.GetGeneratorNamesOfPowerPlant(id);
        foreach (var generator in generators)
        {
            var generatorPowers = await GetGeneratorPower(generator, timeStamps[0], timeStamps[1]);
            if (generatorPowers is null) continue;
            foreach (var generatorPower in generatorPowers)
            {
                var generatorPowerTimePointLocal = generatorPower.TimePoint;
                var matches = powerStamps.Where(x => x.Start == generatorPowerTimePointLocal).ToList();
                foreach (var match in matches)
                {
                    match.Power += generatorPower.Power;
                }
            }
        }
        return powerStamps;
    }

    private string GetApiToken()
    {
        var securityToken = "";
        try
        {
            securityToken = _configuration["APIToken"];
        }
        catch (IOException ioException)
        {
            Console.WriteLine(ioException.Message);
        }
        return securityToken;
    }
}
