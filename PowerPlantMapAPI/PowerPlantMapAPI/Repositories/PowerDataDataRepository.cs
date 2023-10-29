using Dapper;
using System.Data;
using System.Data.SqlClient;
using PowerPlantMapAPI.Data.Dto;

namespace PowerPlantMapAPI.Repositories;

public class PowerDataDataRepository : IPowerDataRepository
{
    private readonly string _connectionString;
    public PowerDataDataRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString(configuration["ConnectionStringToBeUsed"]);
    }
    
    public async Task<List<string>> GetGeneratorNames()
    {
        await using var connection = new SqlConnection(_connectionString);
        var generators = (List<string>)await connection.QueryAsync<string>
            ("GetGeneratorNames", commandType: CommandType.StoredProcedure);
        return generators;
    }

    public async Task<List<string?>> GetGeneratorNamesOfPowerPlant(string powerPlant)
    {
        await using var connection = new SqlConnection(_connectionString);
        var parameters = new { PowerPlantId = powerPlant };
        List<string?> generators = ((List<string>)await connection.QueryAsync<string>
            ("GetGeneratorsOfPowerPlant", parameters, commandType: CommandType.StoredProcedure))!;
        return generators;
    }

    public async Task<List<PowerPlantDataDto>> GetDataOfPowerPlants()
    {
        await using var connection = new SqlConnection(_connectionString);
        return (List<PowerPlantDataDto>)await connection.QueryAsync<PowerPlantDataDto>
            ("[GetDataOfPowerPlants]", CommandType.StoredProcedure);
    }

    public async Task<PowerPlantDataDto> GetDataOfPowerPlant(string id)
    {
        await using var connection = new SqlConnection(_connectionString);
        var parameters = new { Id = id };
        var basicsOfPowerPlant = (List<PowerPlantDataDto>)await connection.QueryAsync<PowerPlantDataDto>
            ("[GetDataOfPowerPlant]", parameters, commandType: CommandType.StoredProcedure);
        return basicsOfPowerPlant[0];
    }
    
    public async Task<List<DateTime>> GetLastDataTime()
    {
        await using var connection = new SqlConnection(_connectionString);
        var lastData = (List<DateTime>)await connection.QueryAsync<DateTime>
            ("GetLastDataTime", commandType: CommandType.StoredProcedure);
        return lastData;
    }
    
    public async Task<List<PastActivityDto>> GetPastActivity(string? generator, DateTime start, DateTime end)
    {
        await using var connection = new SqlConnection(_connectionString);
        var parameters = new { GeneratorId = generator, PeriodStart = start, PeriodEnd = end };
        var pastActivity = (List<PastActivityDto>)await connection.QueryAsync<PastActivityDto>
            ("GetPastActivity", parameters, commandType: CommandType.StoredProcedure);
        return pastActivity;
    }

    public async Task AddPastActivity(string generatorId, DateTime startDateTime, int power)
    {
        await using var connection = new SqlConnection(_connectionString);
        var parameters = new { GeneratorId = generatorId, PeriodStart = startDateTime, ActualPower = power };
        try
        {
            await connection.QueryAsync("AddPastActivity", parameters, commandType: CommandType.StoredProcedure);
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine("While inserting into the db: ", exception.Message);
        }
    }
}