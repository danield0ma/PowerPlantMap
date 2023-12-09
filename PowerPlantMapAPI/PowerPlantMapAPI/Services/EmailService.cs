using System.Globalization;
using System.Text;
using Microsoft.Extensions.Primitives;
using PowerPlantMapAPI.Data;
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
    private readonly IEmailSubscriptionsRepository _emailSubscriptionsRepository;

    public EmailService(IConfiguration configuration, IPowerDataRepository powerDataRepository,
        IDateHelper dateHelper, IEmailSubscriptionsRepository emailSubscriptionsRepository)
    {
        _configuration = configuration;
        _powerDataRepository = powerDataRepository;
        _dateHelper = dateHelper;
        _emailSubscriptionsRepository = emailSubscriptionsRepository;
    }

    public async Task<string?> GenerateAndSendDailyStatisticsInEmail(
        IEnumerable<CompactPowerPlantStatistics> compactPowerPlantStatistics, CountryStatisticsDtoWrapper countryStatistics,
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

        var style =
            "#frame {border-style: solid; border-width: thin; border-color: #dadce0; border-radius: 8px; padding: 40px 20px; margin: 40px 20px; text-align: left}" +
            "#outer {padding-bottom: 20px; max-width: 850px; min-width: 600px; margin: auto; }" +
            "a {text-decoration: none; color: #1a73e8; font-weight: bold; margin-top: 18px;}" +
            "p {font-size: 20px;}" +
            "td {padding: 0 10px;}";
        var body = new StringBuilder($"<html><head><style>{style}</style></head><body><table id=\"outer\"><tbody><tr><td><div id=\"frame\"><div style=\"text-align: center;\"><h1>Napi erőműstatisztika - {date}</h1></div>");

        body.Append("<h2 style=\"text-align: center;\">\nErőmű statisztika</h2><table style=\"margin: auto;\">");
        body.Append(
            "<thead><td><p>Erőmű</p></td><td><p>Termelt energia</p></td><td><p>Átlagos teljesítmény (Max teljesítmény)</p></td><td><p>Átlagos kihasználtság</p></td></thead>");

        foreach (var powerPlantStatistics in compactPowerPlantStatistics)
        {
            body.Append($"<tr><td><p>{powerPlantStatistics.PowerPlantDescription}</p></td><td><p>{Format(powerPlantStatistics.GeneratedEnergy)} MWh</p></td>" +
                        $"<td><p>{Format(powerPlantStatistics.AveragePower)} MW ({Format(powerPlantStatistics.MaxPower)} MW)</p></td><td><p>{Format(powerPlantStatistics.AverageUsage)}%</p></td></tr>");
        }
        body.Append("</table>");
        body.Append("<h2 style=\\\"text-align: center;\\\">Hazai termelés megoszlása az erőművek között grafikonon</h2>");
        var src =
            "https://image-charts.com/chart.js/2.8.0?bkg=white&c=%7B%0A%20%20%22type%22%3A%20%22pie%22%2C%0A%20%20%22data%22%3A%20%7B%0A%20%20%20%20%22datasets%22%3A%20%5B%0A%20%20%20%20%20%20%7B%0A%20%20%20%20%20%20%20%20%22data%22%3A%20%5B99741.25%2C%2060939%5D%2C%0A%20%20%20%20%20%20%7D%0A%20%20%20%20%5D%2C%0A%20%20%20%20%22labels%22%3A%20%5B%22Hazai%20termel%C3%A9s%22%2C%20%22Import-export%20szald%C3%B3%22%5D%0A%20%20%7D%0A%7D";
        body.Append($"<img src=\"{src}\" />");
        
        // var powerPlants = await _powerDataRepository.GetDataOfPowerPlants();
        // var filteredPowerPlants = powerPlants.Where(x => x.IsCountry == false).ToList();
        // var generatedEnergyByPowerPlants = new List<double>();
        // foreach (var powerPlant in filteredPowerPlants)
        // {
        //     var blocData = new StringBuilder("<ul>");
        //     var statisticsOfCurrentPowerPlant = compactPowerPlantStatistics.Data?
        //         .Where(x => x.PowerPlantId == powerPlant.PowerPlantId);
        //     var blocs = statisticsOfCurrentPowerPlant
        //         .GroupBy(x => x.BlocId)
        //         .Select(group => group.First()).ToList();
        //
        //     foreach (var bloc in blocs)
        //     {
        //         var generatorData = new StringBuilder("<ul>");
        //         var generatorsOfBloc = statisticsOfCurrentPowerPlant.Where(x => x.BlocId == bloc.BlocId).ToList();
        //         foreach (var generator in generatorsOfBloc)
        //         {
        //             generatorData.Append($"<li>{generator.GeneratorId}: {Format(generator.AverageUsage)}% - ");
        //             generatorData.Append(
        //                 $"{Format(generator.AveragePower)} MW/{Format(generator.MaxPower)} MW -> {Format(generator.GeneratedEnergy)}  MWh</li>");
        //         }
        //
        //         var blocCurrentAvgPower = generatorsOfBloc.Select(x => x.AveragePower).Sum();
        //         var blocMaxPower = generatorsOfBloc.Select(x => x.MaxPower).Sum();
        //         var blocGeneratedEnergy = generatorsOfBloc.Select(x => x.GeneratedEnergy).Sum();
        //         var avgUsageOfBloc = Math.Round(blocCurrentAvgPower / blocMaxPower * 100, 3);
        //         blocData.Append($"<li>{bloc.BlocId}: {Format(avgUsageOfBloc)}% - ");
        //         blocData.Append(
        //             $"{Format(Math.Round(blocCurrentAvgPower, 3))} MW/{Format(blocMaxPower)} MW -> {Format(blocGeneratedEnergy)}  MWh</li>");
        //         if (generatorsOfBloc.Count > 1)
        //         {
        //             blocData.Append($"{generatorData}</ul>");
        //         }
        //     }
        //
        //     var powerPlantMaxPower = statisticsOfCurrentPowerPlant.Select(x => x.MaxPower).Sum();
        //     var powerPlantCurrentAvgPower = statisticsOfCurrentPowerPlant.Select(x => x.AveragePower).Sum();
        //     var powerPlantGeneratedEnergy = statisticsOfCurrentPowerPlant.Select(x => x.GeneratedEnergy).Sum();
        //     var avgUsageOfPowerPlant = Math.Round(powerPlantCurrentAvgPower / powerPlantMaxPower * 100, 3);
        //     body.Append($"<li>{powerPlant.Description}: {Format(avgUsageOfPowerPlant)}% - ");
        //     body.Append(
        //         $"{Format(Math.Round(powerPlantCurrentAvgPower, 3))} MW/{Format(powerPlantMaxPower)} MW -> ");
        //     body.Append($"{Format(powerPlantGeneratedEnergy)}  MWh</li>");
        //     if (blocs.Count > 1 ||
        //         compactPowerPlantStatistics.Data?.Where(x => x.PowerPlantId == powerPlant.PowerPlantId).ToList().Count > 1)
        //     {
        //         body.Append($"{blocData}</ul>");
        //     }
        //
        //     generatedEnergyByPowerPlants.Append(powerPlantGeneratedEnergy);
        // }
        //
        // var generatedEnergySum = compactPowerPlantStatistics.Data!
        //     .Where(x => filteredPowerPlants.Select(y => y.PowerPlantId).Contains(x.PowerPlantId))
        //     .Select(z => z.GeneratedEnergy).Sum();
        // body.Append($"</ul><h3>Összes termelt energia: {Format(generatedEnergySum)}  MWh</h3>");

        body.Append("<h2 style=\"text-align: center;\">\nImport-Export statisztika</h2><table style=\"margin: auto;\">");
        body.Append(
            "<thead><td><p>Ország</p></td><td><p>Importált energia</p></td><td><p>Exportált energia</p></td><td><p>Szaldó</p></td></thead>");
        var sums = new List<double>();
        foreach (var country in countryStatistics.Data!)
        {
            if (country.CountryId is null) continue;
            var sum = country.ImportedEnergy - country.ExportedEnergy;
            body.Append(
                $"<tr><td><p>{country.CountryName}</p></td><td><p>{Format(country.ImportedEnergy)}  MWh</p></td><td><p>{Format(country.ExportedEnergy)}  MWh</p></td>");
            body.Append($"<td><p>{Format(sum)}  MWh</p></td></tr>");
            sums.Add(sum);
        }

        var importSum = countryStatistics.Data.Select(x => x.ImportedEnergy).Sum();
        var exportSum = countryStatistics.Data.Select(x => x.ExportedEnergy).Sum();

        body.Append($"<tr><td><p>Összesen</p></td><td><p>{Format(importSum)}  MWh</p></td><td><p>{Format(exportSum)}  MWh</p></td>");
        body.Append($"<td><strong><p>{Format(importSum - exportSum)}  MWh</p></strong></td></tr></table>");
        body.Append("<h2 style=\\\"text-align: center;\\\">Import-export megoszlása grafikonon</h2>");
        
        var barCahrtUrl = new StringBuilder();
        barCahrtUrl.Append("https://image-charts.com/chart.js/2.8.0?bkg=white&c=%7B%0A%20%20%22type%22%3A%20%22bar%22%2C%0A%20%20%22data%22%3A%20%7B%0A%20%20%20%20%22datasets%22%3A%20%5B%0A%20%20%20%20%20%20%7B%0A%20%20%20%20%20%20%20%20%22data%22%3A%20%5B"); 
        
        Console.WriteLine("sums");
        foreach (var (sum, index) in sums.Select((value, index) => (value, index)))
        {
            Console.WriteLine(sum);
            barCahrtUrl.Append(sum);
            if (index != sums.Count - 1)
            {
                barCahrtUrl.Append("%2C%20");
            }
        }
        Console.WriteLine(barCahrtUrl);
        barCahrtUrl.Append(
            "%5D%2C%0A%20%20%20%20%20%20%20%20%22backgroundColor%22%3A%20%5B%0A%20%20%20%20%20%20%20%20%20%20%22rgb%28216%2C%200%2C%2039%29%22%2C%0A%20%20%20%20%20%20%20%20%20%20%22rgb%2823%2C%2023%2C%20150%29%22%2C%0A%20%20%20%20%20%20%20%20%20%20%22rgb%28191%2C%20159%2C%2017%29%22%2C%0A%20%20%20%20%20%20%20%20%20%20%22rgb%280%2C%20139%2C%2027%29%22%2C%0A%20%20%20%20%20%20%20%20%20%20%22rgb%280%2C%200%2C%200%29%22%2C%0A%20%20%20%20%20%20%20%20%20%20%22rgb%280%2C%2082%2C%20180%29%22%2C%0A%20%20%20%20%20%20%20%20%20%20%22rgb%28255%2C%20218%2C%2068%29%22%0A%20%20%20%20%20%20%20%20%5D%2C%0A%20%20%20%20%20%20%20label%3A%20%22%22%0A%20%20%20%20%20%20%7D%0A%20%20%20%20%5D%2C%0A%20%20%20%20%22labels%22%3A%20%5B%22Ausztria%22%2C%20%22Horv%C3%A1torsz%C3%A1g%22%2C%20%22Rom%C3%A1nia%22%2C%20%22Szlov%C3%A9nia%22%2C%20%20%22Szerbia%22%2C%20%22Szlov%C3%A1kia%22%2C%20%22Ukrajna%22%5D%0A%20%20%7D%0A%7D");
        body.Append($"<div style=\"text-align: center; margin: 30px;\"><img src={barCahrtUrl.Replace(",", ".")} width=\"500px\" /></div>");
        
        body.Append("<h2 style=\\\"text-align: center;\\\">Hazai termelés és import-export megoszlása grafikonon</h2>");
        var pieChartUrl = "https://image-charts.com/chart.js/2.8.0?bkg=white&c=%7B%0A%20%20%22type%22%3A%20%22pie%22%2C%0A%20%20%22data%22%3A%20%7B%0A%20%20%20%20%22datasets%22%3A%20%5B%0A%20%20%20%20%20%20%7B%0A%20%20%20%20%20%20%20%20%22data%22%3A%20%5B" +
           compactPowerPlantStatistics.Select(x => x.GeneratedEnergy).Sum() + "%2C%20" +
           (importSum - exportSum) + 
           "%5D%2C%0A%20%20%20%20%20%20%7D%0A%20%20%20%20%5D%2C%0A%20%20%20%20%22labels%22%3A%20%5B%22Hazai%20termel%C3%A9s%22%2C%20%22Import-export%20szald%C3%B3%22%5D%0A%20%20%7D%2C%0A%7D";
        body.Append($"<div style=\"text-align: center; margin: 30px;\"><img src={pieChartUrl} width=\"500px\"; /></div>");
        
        var recipients = Get();
        var msg = "";
        foreach (var recipient in recipients!)
        {
            var emailBody = new StringBuilder(body.ToString());
            var unsubscribeUrl = $"https://powerplantmap.tech:5001/api/EmailSubscriptions/Delete?email={recipient.Id}";
            emailBody.Append($"<div style=\"text-align: center;\"><a href={unsubscribeUrl}>Leiratkozás</a></div></div></td></tr></tbody></table></body></html>");
            msg += SendEmail(recipient.Email, $"Napi erőműstatisztika - {date}", emailBody.ToString());
        }
        
        return msg;
    }

    public List<EmailSubscriptionModel>? Get()
    {
        return _emailSubscriptionsRepository.Get();
    }

    public EmailSubscriptionModel? GetById(Guid id)
    {
        return _emailSubscriptionsRepository.GetById(id);
    }
    
    public EmailSubscriptionModel? GetByEmail(string email)
    {
        return _emailSubscriptionsRepository.GetByEmail(email);
    }
    
    public bool Add(string email)
    {
        if (_emailSubscriptionsRepository.GetByEmail(email) is not null)
        {
            throw new ArgumentException("Email already exists.");
        }
        
        var newSubscription = new EmailSubscriptionModel()
        {
            Created = DateTime.UtcNow,
            Email = email
        };
        _emailSubscriptionsRepository.Add(newSubscription);
        return true;
    }

    public void Update(string oldEmail, string newEmail)
    {
        var newSubscription = _emailSubscriptionsRepository.GetByEmail(newEmail);
        if (newSubscription is not null)
        {
            throw new ArgumentException("New Email already exists in DB!");
        }
        
        var subscription = _emailSubscriptionsRepository.GetByEmail(oldEmail);
        if (subscription is null)
        {
            throw new ArgumentException("Old Email not found");
        }
        _emailSubscriptionsRepository.Update(subscription!.Id, newEmail);
    }

    public void Delete(Guid id)
    {
        var subscription = _emailSubscriptionsRepository.GetById(id);
        if (subscription is null)
        {
            throw new ArgumentException("Email not found");
        }
        _emailSubscriptionsRepository.Delete(subscription);
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
        
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
           Console.WriteLine($"Error while sending the email! {response.ErrorMessage}");
           return $"Error while sending the email! {response.ErrorMessage}";
        }
        
        return "Emails sent successfully!";
    }
    
    private static string Format(double number)
    {
        return number.ToString("N", CultureInfo.CurrentCulture).TrimEnd('0').TrimEnd(',');
    }
}
