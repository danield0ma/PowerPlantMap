using System.Xml.Linq;
using PowerPlantMapAPI.Repositories;

namespace PowerPlantMapAPI.Helpers;

public class XmlHelper: IXmlHelper
{
    private readonly IPowerRepository _powerRepository;
    private readonly IPowerHelper _powerHelper;

    public XmlHelper(IPowerRepository repository, IPowerHelper powerHelper)
    {
        _powerRepository = repository;
        _powerHelper = powerHelper;
    }
    
    public async Task GetPowerPlantData(string docType, IReadOnlyList<DateTime> timeStampsUtc)
    {
        if (docType is "A73" or "A75")
        {
            try
            {
                var generators = await _powerRepository.GetGeneratorNames();

                var document = XDocument.Parse(await _powerHelper.ApiQuery(docType, timeStampsUtc[0], timeStampsUtc[1]));
                var ns = document.Root?.Name.Namespace;

                if (document.Root is not null && ns is not null && document.Root?.Elements(ns + "TimeSeries") is not null)
                {
                    foreach (var timeSeries in document.Root?.Elements(ns + "TimeSeries")!)
                    {
                        var startTimePointUtc = timeStampsUtc[0];
                        var generatorName = docType switch
                        {
                            "A73" => timeSeries.Element(ns + "MktPSRType")
                                ?.Element(ns + "PowerSystemResources")
                                ?.Element(ns + "name")
                                ?.Value,
                            "A75" => timeSeries.Element(ns + "MktPSRType")?.Element(ns + "psrType")?.Value,
                            _ => ""
                        };

                        if (generatorName is null || !generators.Contains(generatorName)) continue;
                        var period = timeSeries.Element(ns + "Period");
                        if (period is null) continue;
                        foreach (var point in period.Elements(ns + "Point"))
                        {
                            var currentPower = Convert.ToInt32(point.Element(ns + "quantity")?.Value);
                            await _powerRepository.AddPastActivity(generatorName, startTimePointUtc, currentPower);
                            startTimePointUtc = startTimePointUtc.AddMinutes(15);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        else
        {
            throw new NotImplementedException("Unimplemented DocumentType was given");
        }
    }

    public async Task GetImportAndExportData(string homeCountry, IReadOnlyList<DateTime> timeStampsUtc)
    {
        List<string> neighbourCountries = new()
        {
            "10YSK-SEPS-----K",
            "10YAT-APG------L",
            "10YSI-ELES-----O",
            "10YHR-HEP------M",
            "10YCS-SERBIATSOV",
            "10YRO-TEL------P",
            "10Y1001C--00003F"
        };

        List<string> problematicCountries = new()
        {
            "10YSK-SEPS-----K",
            "10YHR-HEP------M",
            "10YCS-SERBIATSOV",
            "10Y1001C--00003F"
        };

        foreach (var countryCode in neighbourCountries)
        {
            var startTimePointUtc = timeStampsUtc[0];
            var getStartTimeUtc = timeStampsUtc[0];
            var getEndTimeUtc = timeStampsUtc[1];
            
            if (problematicCountries.Contains(countryCode))
            {
                getStartTimeUtc = getStartTimeUtc.AddMinutes(getStartTimeUtc.Minute * -1);
                getEndTimeUtc = getEndTimeUtc.AddMinutes(getEndTimeUtc.Minute * -1);
            }

            try
            {
                var importedEnergyData = XDocument.Parse(await _powerHelper.ApiQuery("A11", getStartTimeUtc, getEndTimeUtc, homeCountry, countryCode));
                var exportedEnergyData = XDocument.Parse(await _powerHelper.ApiQuery("A11", getStartTimeUtc, getEndTimeUtc, countryCode, homeCountry));

                var importNameSpace = importedEnergyData.Root?.Name.Namespace;
                var exportNameSpace = exportedEnergyData.Root?.Name.Namespace;

                if (importNameSpace is not null && exportNameSpace is not null)
                {
                   var importTimeSeries = importedEnergyData.Root?.Element(importNameSpace + "TimeSeries");
                   var exportTimeSeries = exportedEnergyData.Root?.Element(exportNameSpace + "TimeSeries");

                   if (importTimeSeries?.Elements() is not null && exportTimeSeries?.Elements() is not null)
                   {
                       var importPeriod = importTimeSeries.Element(importNameSpace + "Period");
                       var exportPeriod = exportTimeSeries.Element(exportNameSpace + "Period");

                       if (importPeriod?.Elements(importNameSpace + "Point") is not null && exportPeriod?.Elements(exportNameSpace + "Point") is not null)
                       {
                           for (var i = 0; i < importPeriod.Elements(importNameSpace + "Point").Count(); i++)
                           {
                               var importPoint = importPeriod.Elements(importNameSpace + "Point").ToList()[i];
                               var exportPoint = exportPeriod.Elements(importNameSpace + "Point").ToList()[i];

                               var importValue = Convert.ToInt32(importPoint.Element(importNameSpace + "quantity")?.Value);
                               var exportValue = Convert.ToInt32(exportPoint.Element(exportNameSpace + "quantity")?.Value);
                               exportValue *= -1;

                               var currentPower = importValue + exportValue;

                               var numberOfTimesTheValueHasToBeSaved = 1;
                               if (problematicCountries.Contains(countryCode))
                               {
                                   numberOfTimesTheValueHasToBeSaved = 4;
                               }

                               for (var j = 0; j < numberOfTimesTheValueHasToBeSaved; j++)
                               {
                                   await _powerRepository.AddPastActivity(countryCode, startTimePointUtc, currentPower);
                                   startTimePointUtc = startTimePointUtc.AddMinutes(15);
                               }
                           }
                       }
                   } 
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}