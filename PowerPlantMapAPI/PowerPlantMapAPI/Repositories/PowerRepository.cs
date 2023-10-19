﻿﻿using Dapper;
using System.Data;
using System.Data.SqlClient;
 using PowerPlantMapAPI.Models.DTO;

 namespace PowerPlantMapAPI.Repositories
{
    public class PowerRepository : IPowerRepository
    {
        private readonly string _connectionString;
        public PowerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString(configuration["ConnectionStringToBeUsed"]);
        }
        
        public async Task<List<string>> GetPowerPlantNames()
        {
            await using var connection = new SqlConnection(_connectionString);
            var powerPlants = (List<string>)await connection.QueryAsync<string>
                ("GetPowerPlants", commandType: CommandType.StoredProcedure);
            return powerPlants;
        }
        
        public async Task<List<string>> GetGeneratorNames()
        {
            await using var connection = new SqlConnection(_connectionString);
            var generators = (List<string>)await connection.QueryAsync<string>
                ("GetGenerators", commandType: CommandType.StoredProcedure);
            return generators;
        }

        public async Task<List<string?>> GetGeneratorNamesOfPowerPlant(string powerPlant)
        {
            await using var connection = new SqlConnection(_connectionString);
            var parameters = new { PPID = powerPlant };
            List<string?> generators = ((List<string>)await connection.QueryAsync<string>
                ("GetGeneratorsOfPowerPlant", parameters, commandType: CommandType.StoredProcedure))!;
            return generators;
        }

        public async Task<List<PowerPlantDataDto>> GetDataOfPowerPlants()
        {
            await using var connection = new SqlConnection(_connectionString);
            return (List<PowerPlantDataDto>)await connection.QueryAsync<PowerPlantDataDto>
                ("[PowerPlantBasics]", CommandType.StoredProcedure);
        }

        public async Task<PowerPlantDataDto> GetDataOfPowerPlant(string id)
        {
            await using var connection = new SqlConnection(_connectionString);
            var parameters = new { id };
            var basicsOfPowerPlant = (List<PowerPlantDataDto>)await connection.QueryAsync<PowerPlantDataDto>
                ("[GetBasicsOfPowerPlant]", parameters, commandType: CommandType.StoredProcedure);
            return basicsOfPowerPlant[0];
        }

        public async Task<List<PowerPlantDetailsDto>> GetPowerPlantDetails(string id)
        {
            await using var connection = new SqlConnection(_connectionString);
            var parameters = new { PowerPlantID = id };
            return (List<PowerPlantDetailsDto>)await connection.
                QueryAsync<PowerPlantDetailsDto>("GetPowerPlantDetails",
                parameters, commandType: CommandType.StoredProcedure);
        }
        
        public async Task<List<DateTime>> GetLastDataTime()
        {
            await using var connection = new SqlConnection(_connectionString);
            var parameter = new { PPID = "PKS" };
            var lastData = (List<DateTime>)await connection.QueryAsync<DateTime>
                ("GetLastDataTime", parameter, commandType: CommandType.StoredProcedure);
            return lastData;
        }
        
        public async Task<List<PastActivityDto>> GetPastActivity(string? generator, DateTime start, DateTime end)
        {
            await using var connection = new SqlConnection(_connectionString);
            var parameters = new { GID = generator, start, end };
            var pastActivity = (List<PastActivityDto>)await connection.QueryAsync<PastActivityDto>
                ("GetPastActivity", parameters, commandType: CommandType.StoredProcedure);
            return pastActivity;
        }

        public async Task AddPastActivity(string generatorId, DateTime startDateTime, int power)
        {
            await using var connection = new SqlConnection(_connectionString);
            var parameters = new { GID = generatorId, start = startDateTime, power };
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
}