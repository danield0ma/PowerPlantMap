using System;
using System.Globalization;
using System.Text;
using PowerPlantMapAPI.Helpers;
using PowerPlantMapAPI.Models.DTO;
using PowerPlantMapAPI.Repositories;

namespace PowerPlantMapAPI.Services;

public class StatisticsService : IStatisticsService
{
    private readonly IPowerRepository _powerRepository;
    private readonly IDateHelper _dateHelper;
    private readonly IEmailService _emailService;
    
    public StatisticsService(IPowerRepository powerRepository, IDateHelper dateHelper, IEmailService emailService)
    {
        _powerRepository = powerRepository;
        _dateHelper = dateHelper;
        _emailService = emailService;
    }
    
    public async Task<List<DailyStatisticsDto>> CreateAndSendDailyStatistics()
    {
        List<DailyStatisticsDto> statistics = new();
        var startAndEndTimeOfDailyStatistics = _dateHelper.GetStartAndEndTimeOfDailyStatistics();
        var date = startAndEndTimeOfDailyStatistics[0].Year + "." +
                        startAndEndTimeOfDailyStatistics[0].Month + "." +
                        startAndEndTimeOfDailyStatistics[0].Day;
        var start = startAndEndTimeOfDailyStatistics[0];
        var end = startAndEndTimeOfDailyStatistics[1];
        var generatedEnergySum = 0.0;
        var result = new StringBuilder($"<html><body><h3>Napi erőműstatisztika - {date}</h3><ul>");

        // var powerPlants = await _powerRepository.GetPowerPlantNames();
        var powerPlants = await _powerRepository.GetDataOfPowerPlants();
        foreach (var powerPlant in powerPlants.Where(x => x.IsCountry == false).ToList())
        {
            if (powerPlant.PowerPlantId is null) continue;
            var powerPlantDetails = await _powerRepository.GetPowerPlantDetails(powerPlant.PowerPlantId);
            
            var powerPlantMaxPower = 0;
            var powerPlantCurrentAvgPower = 0.0;
            var powerPlantGeneratedEnergy = 0.0;
            var blocData = new StringBuilder("<ul>");
            
            var blocs = powerPlantDetails.
                GroupBy(x => x.BlocId).
                Select(group => group.First()).
                ToList();
            foreach (var bloc in blocs)
            {
                var blocMaxPower = 0;
                var blocCurrentAvgPower = 0.0;
                var blocGeneratedEnergy = 0.0;
                var generatorData = new StringBuilder("<ul>");

                var generators = powerPlantDetails.
                    Where(x => x.BlocId == bloc.BlocId).ToList();
                foreach (var generator in generators)
                {
                    if (generator.GeneratorId == null) continue;
                    var maxPowerOfGenerator = await _powerRepository.GetMaxPowerOfGenerator(generator.GeneratorId);
                    var pastActivity = await _powerRepository.GetPastActivity(generator.GeneratorId, start, end);
                    
                    var avgPowerOfGenerator = Math.Round(pastActivity.Select(x => x.ActualPower).Average(), 3);
                    var generatedEnergyByGenerator = Math.Round(pastActivity.Sum(activity => activity.ActualPower * 0.25), 3);
                    var avgUsageOfGenerator = Math.Round(avgPowerOfGenerator / maxPowerOfGenerator * 100, 3);

                    DailyStatisticsDto stat = new()
                    {
                        GeneratorName = generator.GeneratorId,
                        MaxPower = maxPowerOfGenerator,
                        AveragePower = avgPowerOfGenerator,
                        GeneratedEnergy = generatedEnergyByGenerator,
                        AverageUsage = avgUsageOfGenerator
                    };
                    statistics.Add(stat);
                    generatorData.Append($"<li>{generator.GeneratorId}: {Format(avgUsageOfGenerator)}% - ");
                    generatorData.Append($"{Format(avgPowerOfGenerator)} MW/{Format(maxPowerOfGenerator)} MW -> {Format(generatedEnergyByGenerator)}  MWh</li>");
                    
                    blocMaxPower += maxPowerOfGenerator;
                    blocCurrentAvgPower += Math.Round(avgPowerOfGenerator, 3);
                    blocGeneratedEnergy += Math.Round(generatedEnergyByGenerator, 3);
                }

                var avgUsageOfBloc = Math.Round(blocCurrentAvgPower / blocMaxPower * 100, 3);
                blocData.Append($"<li>{bloc.BlocId}: {Format(avgUsageOfBloc)}% - ");
                blocData.Append($"{Format(Math.Round(blocCurrentAvgPower, 3))} MW/{Format(blocMaxPower)} MW -> {Format(blocGeneratedEnergy)}  MWh</li>");
                if (generators.Count > 1)
                {
                    blocData.Append($"{generatorData}</ul>");
                }
                
                powerPlantMaxPower += blocMaxPower;
                powerPlantCurrentAvgPower += Math.Round(blocCurrentAvgPower, 3);
                powerPlantGeneratedEnergy += Math.Round(blocGeneratedEnergy, 3);
            }
            
            var avgUsageOfPowerPlant = Math.Round(powerPlantCurrentAvgPower / powerPlantMaxPower * 100, 3);
            result.Append($"<li>{powerPlant.Description}: {Format(avgUsageOfPowerPlant)}% - ");
            result.Append($"{Format(Math.Round(powerPlantCurrentAvgPower, 3))} MW/{Format(powerPlantMaxPower)} MW ->;");
            result.Append($"{Format(powerPlantGeneratedEnergy)}  MWh</li>");
            if (blocs.Count > 1 ||
                powerPlantDetails.Where(x => x.BlocId == blocs[0].BlocId).ToList().Count > 1)
            {
                result.Append($"{blocData}</ul>");
            }
            generatedEnergySum += powerPlantGeneratedEnergy;
        }
        result.Append($"</ul><h3>Összes termelt energia: {Format(generatedEnergySum)}  MWh</h3>");

        var importSum = 0.0;
        var exportSum = 0.0;
        result.Append($"<h3>\nImport-Export statisztika - {date}</h3><table>");
        result.Append("<thead><td>Ország</td><td>Importált energia</td><td>Exportált energia</td><td>Szaldó</td></thead>");
        foreach (var country in powerPlants.Where(x => x.IsCountry).ToList())
        {
            if (country.PowerPlantId is null) continue;
            var generator = await _powerRepository.GetGeneratorNamesOfPowerPlant(country.PowerPlantId);
            var pastActivity = await _powerRepository.GetPastActivity(generator[0], start, end);
            var importedEnergy = Math.Round(pastActivity.
                Where(x => x.ActualPower > 0).
                Sum(x => x.ActualPower * 0.25), 3);
            var exportedEnergy = Math.Abs(Math.Round(pastActivity.
                Where(x => x.ActualPower < 0).
                Sum(x => x.ActualPower * 0.25), 3));
            
            result.Append($"<tr><td>{country.Name}</td><td>{Format(importedEnergy)}  MWh</td><td>{Format(exportedEnergy)}  MWh</td>");
            result.Append($"<td>{Format(importedEnergy - exportedEnergy)}  MWh</td></tr>");

            importSum += importedEnergy;
            exportSum += exportedEnergy;
        }
        
        result.Append($"<tr><td>Összesen</td><td>{Format(importSum)}  MWh</td><td>{Format(exportSum)}  MWh</td>");
        result.Append($"<td><strong>{Format(importSum - exportSum)}  MWh</strong></td></tr>");
        result.Append("</table></body></html>");
        _emailService.SendEmail("daniel2.doma@gmail.com",
                            $"Napi erőműstatisztika - {date}",
                            result.ToString());
        return statistics;
    }

    private static string Format(double number)
    {
        return number.ToString("N", CultureInfo.CurrentCulture).TrimEnd('0').TrimEnd(',');
    }

    // private async Task UpdateMaxPowerCapacitiesOfPowerPlants()
    // {
    //     
    // }
}
