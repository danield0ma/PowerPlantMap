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
}