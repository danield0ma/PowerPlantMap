using Dapper;
using PowerPlantMapAPI.Models;
using System.Data;
using System.Data.SqlClient;

namespace PowerPlantMapAPI.Repositories
{
    public class PowerRepository : IPowerRepository
    {
        private readonly string _connectionString;
        public PowerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString(configuration["ConnectionStringToBeUsed"]);
        }
        
        public async Task<List<string>> GetPowerPlants()
        {
            await using var connection = new SqlConnection(_connectionString);
            var powerPlants = (List<string>)await connection.QueryAsync<string>
                ("GetPowerPlants", commandType: CommandType.StoredProcedure);
            return powerPlants;
        }
        
        public async Task<List<string>> GetGenerators()
        {
            await using var connection = new SqlConnection(_connectionString);
            var generators = (List<string>)await connection.QueryAsync<string>
                ("GetGenerators", commandType: CommandType.StoredProcedure);
            return generators;
        }

        public async Task<int> GetMaxPowerOfGenerator(string generator)
        {
            await using var connection = new SqlConnection(_connectionString);
            var maxPowerOfGenerator = (List<int>)await connection.QueryAsync<int>
                ("GetMaxPowerOfGenerator", commandType: CommandType.StoredProcedure);
            return maxPowerOfGenerator[0];
        }

        public async Task<List<string>> GetGeneratorsOfPowerPlant(string powerPlant)
        {
            await using var connection = new SqlConnection(_connectionString);
            var parameters = new { PPID = powerPlant };
            var generators = (List<string>)await connection.QueryAsync<string>
                ("GetGeneratorsOfPowerPlant", parameters, commandType: CommandType.StoredProcedure);
            return generators;
        }

        public async Task<List<PowerPlantDataModel>> GetPowerPlantBasics()
        {
            await using var connection = new SqlConnection(_connectionString);
            return (List<PowerPlantDataModel>)await connection.QueryAsync<PowerPlantDataModel>
                ("[PowerPlantBasics]", CommandType.StoredProcedure);
        }

        public async Task<PowerPlantDataModel> GetBasicsOfPowerPlant(string id)
        {
            await using var connection = new SqlConnection(_connectionString);
            var parameters = new { id };
            var basicsOfPowerPlant = (List<PowerPlantDataModel>)await connection.QueryAsync<PowerPlantDataModel>
                ("[GetBasicsOfPowerPlant]", parameters, commandType: CommandType.StoredProcedure);
            return basicsOfPowerPlant[0];
        }

        public async Task<List<PowerPlantDetailsModel>> GetPowerPlantDetails(string id)
        {
            await using var connection = new SqlConnection(_connectionString);
            var parameters = new { PowerPlantID = id };
            return (List<PowerPlantDetailsModel>)await connection.
                QueryAsync<PowerPlantDetailsModel>("GetPowerPlantDetails",
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
        
        public async Task<List<PastActivityModel>> GetPastActivity(string generator, DateTime start, DateTime end)
        {
            await using var connection = new SqlConnection(_connectionString);
            var parameters = new { GID = generator, start, end };
            var pastActivity = (List<PastActivityModel>)await connection.QueryAsync<PastActivityModel>
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
