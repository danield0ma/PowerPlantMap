using Dapper;
using System.Data;
using System.Data.SqlClient;
using PowerPlantMapAPI.Data.Dto;

namespace PowerPlantMapAPI.Repositories;

public class PowerPlantRepository : IPowerPlantRepository
{
    private readonly string _connectionString;

    public PowerPlantRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("PPM");
    }
    
    public async Task<IEnumerable<PowerPlantDataDto?>> Get()
    {
        await using var connection = new SqlConnection(_connectionString);
        var dataOfPowerPlants = (List<PowerPlantDataDto>)await connection.QueryAsync<PowerPlantDataDto>
            ("GetDataOfPowerPlants", commandType: CommandType.StoredProcedure);
        return dataOfPowerPlants;
    }

    public async Task<PowerPlantDataDto?> GetById(string id)
    {
        await using var connection = new SqlConnection(_connectionString);
        var parameters = new { id };
        var dataOfPowerPlant = await connection.QueryAsync<PowerPlantDataDto>
            ("GetDataOfPowerPlant", parameters, commandType: CommandType.StoredProcedure);
        return dataOfPowerPlant.FirstOrDefault();
    }

    public async Task<bool> AddPowerPlant(PowerPlantDataDto powerPlantToBeAdded)
    {
        await using var connection = new SqlConnection(_connectionString);
        var parameters = new { powerPlant = powerPlantToBeAdded };
        var numberOfRows = await connection.ExecuteAsync("AddPowerPlant", parameters, commandType: CommandType.StoredProcedure);
        return numberOfRows == 1;
    }
    
    public async Task<bool> DeletePowerPlant(string id)
    {
        await using var connection = new SqlConnection(_connectionString);
        var parameters = new { PowerPlantId = id };
        var numberOfRows = await connection.ExecuteAsync("DeletePowerPlant", parameters, commandType: CommandType.StoredProcedure);
        return numberOfRows == 1;
    }
}