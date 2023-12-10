using Dapper;
using System.Data;
using System.Data.SqlClient;
using PowerPlantMapAPI.Data;
using PowerPlantMapAPI.Data.Dto;

namespace PowerPlantMapAPI.Repositories;

public class PowerPlantRepository : IPowerPlantRepository
{
    private readonly string _connectionString;

    public PowerPlantRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("PPM");
    }
    
    public async Task<List<string>> GetPowerPlantNames()
    {
        await using var connection = new SqlConnection(_connectionString);
        var powerPlants = (List<string>)await connection.QueryAsync<string>
            ("GetPowerPlantNames", commandType: CommandType.StoredProcedure);
        return powerPlants;
    }
    
    public async Task<List<PowerPlantDetailsDto>> GetPowerPlantDetails(string id)
    {
        await using var connection = new SqlConnection(_connectionString);
        var parameters = new { PowerPlantId = id };
        return (List<PowerPlantDetailsDto>)await connection.
            QueryAsync<PowerPlantDetailsDto>("GetPowerPlantDetails",
                parameters, commandType: CommandType.StoredProcedure);
    }
    
    public async Task<List<AllDataOfPowerPlantDto>> GetPowerPlantsModel()
    {
        await using var connection = new SqlConnection(_connectionString);
        return (List<AllDataOfPowerPlantDto>)await connection.
            QueryAsync<AllDataOfPowerPlantDto>("GetPowerPlantsModel", commandType: CommandType.StoredProcedure);
    }
    
    public async Task<List<AllDataOfPowerPlantDto>> GetPowerPlantModel(string id)
    {
        await using var connection = new SqlConnection(_connectionString);
        var parameters = new { PowerPlantId = id };
        return (List<AllDataOfPowerPlantDto>)await connection.
            QueryAsync<AllDataOfPowerPlantDto>("GetPowerPlantModel", parameters, commandType: CommandType.StoredProcedure);
    }
    
    public async Task<bool> AddGenerator(GeneratorDataDto generator)
    {
        await using var connection = new SqlConnection(_connectionString);
        var parameters = new { generator.GeneratorId, generator.MaxCapacity };
        try
        {
            var numberOfRowsAffected = await connection.ExecuteAsync("AddGenerator", parameters, commandType: CommandType.StoredProcedure); 
            return numberOfRowsAffected == 1;
        }
        catch (SqlException e)
        {
            Console.WriteLine(e);
        }
        return false;
    }
    
    public async Task<bool> AddBlocGenerator(string blocId, string generatorId)
    {
        await using var connection = new SqlConnection(_connectionString);
        var parameters = new { BlocId = blocId, GeneratorId = generatorId };
        try
        {
            var numberOfRowsAffected = await connection.ExecuteAsync("AddBlocGenerator", parameters, commandType: CommandType.StoredProcedure);
            return numberOfRowsAffected == 1;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return false;
    }
    
    public async Task<bool> AddBloc(BlocDataDto bloc)
    {
        await using var connection = new SqlConnection(_connectionString);
        var parameters = new
        {
            bloc.BlocId,
            bloc.BlocType,
            bloc.MaxBlocCapacity,
            bloc.CommissionDate
        };
        try
        {
            var numberOfRowsAffected = await connection.ExecuteAsync("AddBloc", parameters, commandType: CommandType.StoredProcedure);
            return numberOfRowsAffected == 1;
        }
        catch (SqlException e)
        {
            Console.WriteLine(e);
        }
        return false;
    }
    
    public async Task<bool> AddPowerPlantBloc(string powerPlantId, string blocId)
    {
        await using var connection = new SqlConnection(_connectionString);
        var parameters = new { PowerPlantId = powerPlantId, BlocId = blocId };
        try
        {
            var numberOfRowsAffected = await connection.ExecuteAsync("AddPowerPlantBloc", parameters, commandType: CommandType.StoredProcedure);
            return numberOfRowsAffected == 1;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return false;
    }
    
    public async Task<bool> AddPowerPlant(PowerPlantDataDto powerPlantToBeAdded)
    {
        await using var connection = new SqlConnection(_connectionString);
        var parameters = new
        {
            powerPlantToBeAdded.PowerPlantId,
            powerPlantToBeAdded.Name,
            powerPlantToBeAdded.Description,
            powerPlantToBeAdded.OperatorCompany,
            powerPlantToBeAdded.Webpage,
            powerPlantToBeAdded.Image,
            powerPlantToBeAdded.Longitude,
            powerPlantToBeAdded.Latitude,
            powerPlantToBeAdded.Color,
            powerPlantToBeAdded.Address,
            powerPlantToBeAdded.IsCountry
        };
        try
        {
            var numberOfRowsAffected = await connection.ExecuteAsync("AddPowerPlant", parameters, commandType: CommandType.StoredProcedure);
            return numberOfRowsAffected == 1;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return false;
    }

    public async Task<bool> DeleteGenerator(string id)
    {
        await using var connection = new SqlConnection(_connectionString);
        var parameters = new { GeneratorId = id };
        var numberOfRows = await connection.ExecuteAsync("DeleteGenerator", parameters, commandType: CommandType.StoredProcedure);
        return numberOfRows > 0;
    }

    public async Task<bool> DeleteBloc(string id)
    {
        await using var connection = new SqlConnection(_connectionString);
        var parameters = new { BlocId = id };
        var numberOfRows = await connection.ExecuteAsync("DeleteBloc", parameters, commandType: CommandType.StoredProcedure);
        return numberOfRows > 0;
    }
    
    public async Task<bool> DeletePowerPlant(string id)
    {
        await using var connection = new SqlConnection(_connectionString);
        var parameters = new { PowerPlantId = id };
        var numberOfRows = await connection.ExecuteAsync("DeletePowerPlant", parameters, commandType: CommandType.StoredProcedure);
        return numberOfRows == 1;
    }
}