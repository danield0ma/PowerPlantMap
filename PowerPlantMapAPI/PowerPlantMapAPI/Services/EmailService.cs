using System.Globalization;
using System.Text;
using PowerPlantMapAPI.Data.Dto;
using PowerPlantMapAPI.Helpers;
using PowerPlantMapAPI.Repositories;
using RestSharp;

namespace PowerPlantMapAPI.Services;

public class EmailService: IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly IPowerDataRepository _powerDataRepository;
    private readonly IDateHelper _dateHelper;

    public EmailService(IConfiguration configuration, IPowerDataRepository powerDataRepository, IDateHelper dateHelper)
    {
        _configuration = configuration;
        _powerDataRepository = powerDataRepository;
        _dateHelper = dateHelper;
    }

    public async Task<string?> GenerateAndSendDailyStatisticsInEmail(
        PowerPlantStatisticsDtoWrapper powerPlantStatistics, CountryStatisticsDtoWrapper countryStatistics,
        DateTime? day = null, DateTime? start = null, DateTime? end = null)
    {
        List<DateTime> startAndEndTimeOfDailyStatistics;
        if (day is null && start is null && end is null)
        {
            startAndEndTimeOfDailyStatistics = _dateHelper.GetStartAndEndTimeOfDailyStatistics();
        }
        else
        {
            startAndEndTimeOfDailyStatistics = await _dateHelper.HandleWhichDateFormatIsBeingUsed(day, start, end);
        }
        
        var date = startAndEndTimeOfDailyStatistics[0].Year + "." +
                   startAndEndTimeOfDailyStatistics[0].Month + "." +
                   startAndEndTimeOfDailyStatistics[0].Day;
        
        var style = "#frame {border-style: solid; border-width: thin; border-color: #dadce0; border-radius: 8px; padding: 40px 20px; margin: 40px 20px; text-align: left}" +
                    "#outer {padding-bottom: 20px; max-width: 850px; min-width: 600px; margin: auto; }";
        var body = new StringBuilder($"<html><head><style>{style}</style></head><body><table id=\"outer\"><tbody><tr><td><div id=\"frame\"><h3>Napi erőműstatisztika - {date}</h3><ul>");

        var powerPlants = await _powerDataRepository.GetDataOfPowerPlants();
        var filteredPowerPlants = powerPlants.Where(x => x.IsCountry == false).ToList();
        foreach (var powerPlant in filteredPowerPlants)
        {
            var blocData = new StringBuilder("<ul>");
            var statisticsOfCurrentPowerPlant = powerPlantStatistics.Data?
                .Where(x => x.PowerPlantId == powerPlant.PowerPlantId);
            var blocs = statisticsOfCurrentPowerPlant
                .GroupBy(x => x.BlocId)
                .Select(group => group.First()).ToList();

            foreach (var bloc in blocs)
            {
                var generatorData = new StringBuilder("<ul>");
                var generatorsOfBloc = statisticsOfCurrentPowerPlant.Where(x => x.BlocId == bloc.BlocId).ToList();
                foreach (var generator in generatorsOfBloc)
                {
                    generatorData.Append($"<li>{generator.GeneratorId}: {Format(generator.AverageUsage)}% - ");
                    generatorData.Append(
                        $"{Format(generator.AveragePower)} MW/{Format(generator.MaxPower)} MW -> {Format(generator.GeneratedEnergy)}  MWh</li>");
                }

                var blocCurrentAvgPower = generatorsOfBloc.Select(x => x.AveragePower).Sum();
                var blocMaxPower = generatorsOfBloc.Select(x => x.MaxPower).Sum();
                var blocGeneratedEnergy = generatorsOfBloc.Select(x => x.GeneratedEnergy).Sum();
                var avgUsageOfBloc = Math.Round(blocCurrentAvgPower / blocMaxPower * 100, 3);
                blocData.Append($"<li>{bloc.BlocId}: {Format(avgUsageOfBloc)}% - ");
                blocData.Append(
                    $"{Format(Math.Round(blocCurrentAvgPower, 3))} MW/{Format(blocMaxPower)} MW -> {Format(blocGeneratedEnergy)}  MWh</li>");
                if (generatorsOfBloc.Count > 1)
                {
                    blocData.Append($"{generatorData}</ul>");
                }
            }

            var powerPlantMaxPower = statisticsOfCurrentPowerPlant.Select(x => x.MaxPower).Sum();
            var powerPlantCurrentAvgPower = statisticsOfCurrentPowerPlant.Select(x => x.AveragePower).Sum();
            var powerPlantGeneratedEnergy = statisticsOfCurrentPowerPlant.Select(x => x.GeneratedEnergy).Sum();
            var avgUsageOfPowerPlant = Math.Round(powerPlantCurrentAvgPower / powerPlantMaxPower * 100, 3);
            body.Append($"<li>{powerPlant.Description}: {Format(avgUsageOfPowerPlant)}% - ");
            body.Append(
                $"{Format(Math.Round(powerPlantCurrentAvgPower, 3))} MW/{Format(powerPlantMaxPower)} MW -> ");
            body.Append($"{Format(powerPlantGeneratedEnergy)}  MWh</li>");
            if (blocs.Count > 1 ||
                powerPlantStatistics.Data?.Where(x => x.PowerPlantId == powerPlant.PowerPlantId).ToList().Count > 1)
            {
                body.Append($"{blocData}</ul>");
            }
        }

        var generatedEnergySum = powerPlantStatistics.Data!
            .Where(x => filteredPowerPlants.Select(y => y.PowerPlantId).Contains(x.PowerPlantId))
            .Select(z => z.GeneratedEnergy).Sum();
        body.Append($"</ul><h3>Összes termelt energia: {Format(generatedEnergySum)}  MWh</h3>");

        body.Append($"<h3>\nImport-Export statisztika - 11.11</h3><table>");
        body.Append(
            "<thead><td>Ország</td><td>Importált energia</td><td>Exportált energia</td><td>Szaldó</td></thead>");
        foreach (var country in countryStatistics.Data!)
        {
            if (country.CountryId is null) continue;
            
            body.Append(
                $"<tr><td>{country.CountryName}</td><td>{Format(country.ImportedEnergy)}  MWh</td><td>{Format(country.ExportedEnergy)}  MWh</td>");
            body.Append($"<td>{Format(country.ImportedEnergy - country.ExportedEnergy)}  MWh</td></tr>");
        }

        var importSum = countryStatistics.Data.Select(x => x.ImportedEnergy).Sum();
        var exportSum = countryStatistics.Data.Select(x => x.ExportedEnergy).Sum();

        body.Append($"<tr><td>Összesen</td><td>{Format(importSum)}  MWh</td><td>{Format(exportSum)}  MWh</td>");
        body.Append($"<td><strong>{Format(importSum - exportSum)}  MWh</strong></td></tr></table>");
        const string url = "https://image-charts.com/chart?chs=190x190&chd=t:60,40&cht=p3&chl=Hello%7CWorld&chan&chf=ps0-0,lg,45,ffeb3b,0.2,f44336,1|ps0-1,lg,45,8bc34a,0.2,009688,1";
        body.Append($"<img src={url} /></div></td></tr></tbody></table></body></html>");
        
        Console.WriteLine(body.ToString());
        var msg = SendEmail("daniel2.doma@gmail.com", $"Napi erőműstatisztika - {date}", body.ToString());
        return msg;
    }
    
    public string SendEmail(string? to, string? subject, string? body)
    {
        var client = new RestClient($"https://api.eu.mailgun.net/v3/{_configuration["Email:MailgunDomain"]}");
        var request = new RestRequest("messages", Method.Post);
        request.AddHeader("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"api:{_configuration["Email:MailgunApiKey"]}"))}");
        request.AddParameter("from", "PowerPlantMap <noreply@powerplantmap.tech>");
        request.AddParameter("to", to);
        request.AddParameter("subject", subject);
        request.AddParameter("html", body);

        var response = client.Execute(request);

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            Console.WriteLine("Email sent successfully!");
            return "Email sent successfully!";
        }
        else
        {
            Console.WriteLine($"Error sending email: {response.ErrorMessage}");
            return "Error";
        }
    }
    
    private static string Format(double number)
    {
        return number.ToString("N", CultureInfo.CurrentCulture).TrimEnd('0').TrimEnd(',');
    }
}