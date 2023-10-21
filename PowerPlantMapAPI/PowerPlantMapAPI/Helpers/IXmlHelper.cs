namespace PowerPlantMapAPI.Helpers;

public interface IXmlHelper
{
    Task GetPowerPlantData(string docType, IReadOnlyList<DateTime> timeStampsUtc);
    Task GetImportAndExportData(string homeCountry, IReadOnlyList<DateTime> timeStampsUtc);
}