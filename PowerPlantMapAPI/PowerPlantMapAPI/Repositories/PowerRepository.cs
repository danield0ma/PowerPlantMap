using Dapper;
using PowerPlantMapAPI.Models;
using System.Data;
using System.Data.SqlClient;

namespace PowerPlantMapAPI.Repositories
{
    public class PowerRepository : IPowerRepository
    {
        private readonly string connectionString;

        public PowerRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString(configuration["ConnectionStringToBeUsed"]);
        }

        public async Task<List<PowerPlantDataModel>> QueryPowerPlantBasics()
        {
            using var connection = new SqlConnection(connectionString);
            return (List<PowerPlantDataModel>)await connection.QueryAsync<PowerPlantDataModel>("[PowerPlantBasics]", CommandType.StoredProcedure);
        }

        public async Task<PowerPlantDataModel> QueryBasicsOfPowerPlant(string id)
        {
            using var connection = new SqlConnection(connectionString);
            var parameters = new { id };
            List<PowerPlantDataModel> PP = (List<PowerPlantDataModel>)await connection.QueryAsync<PowerPlantDataModel>
                ("[GetBasicsOfPowerPlant]", parameters, commandType: CommandType.StoredProcedure);
            return PP[0];
        }

        public async Task<List<PowerPlantDetailsModel>> QueryPowerPlantDetails(string id)
        {
            using var connection = new SqlConnection(connectionString);
            var parameters = new { PowerPlantID = id };
            return (List<PowerPlantDetailsModel>)await connection.
                QueryAsync<PowerPlantDetailsModel>("GetPowerPlantDetails",
                parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<List<PastActivityModel>> QueryPastActivity(string generator, DateTime start, DateTime end)
        {
            using var connection = new SqlConnection(connectionString);
            var parameters = new { GID = generator, start, end };
            List<PastActivityModel> PastActivity = (List<PastActivityModel>)await connection.QueryAsync<PastActivityModel>
                ("GetPastActivity", parameters, commandType: CommandType.StoredProcedure);
            return PastActivity;
        }

        public async Task<List<string>> QueryPowerPlants()
        {
            using var connection = new SqlConnection(connectionString);
            List<string> powerPlants = (List<string>)await connection.QueryAsync<string>("GetPowerPlants", commandType: CommandType.StoredProcedure);
            return powerPlants;
        }

        public async Task<List<string>> QueryGeneratorsOfPowerPlant(string PowerPlant)
        {
            using var connection = new SqlConnection(connectionString);
            var parameters = new { PPID = PowerPlant };
            List<string> generators = (List<string>)await connection.QueryAsync<string>
                ("GetGeneratorsOfPowerPlant", parameters, commandType: CommandType.StoredProcedure);
            return generators;
        }

        public async Task<List<DateTime>> QueryLastDataTime()
        {
            using var connection = new SqlConnection(connectionString);
            var parameter = new { PPID = "PKS" };
            List<DateTime> LastData = (List<DateTime>)await connection.QueryAsync<DateTime>
                ("GetLastDataTime", parameter, commandType: CommandType.StoredProcedure);
            return LastData;
        }

        public async Task<List<string>> QueryGenerators()
        {
            using var connection = new SqlConnection(connectionString);
            List<string> generators = (List<string>)await connection.QueryAsync<string>
                ("GetGenerators", commandType: CommandType.StoredProcedure);
            return generators;
        }

        public async Task InsertData(string generatorId, DateTime startDateTime, int power)
        {
            using var connection = new SqlConnection(connectionString);
            var parameters = new { GID = generatorId, start = startDateTime, power };
            try
            {
                await connection.QueryAsync("AddPastActivity", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception E)
            {
                System.Diagnostics.Debug.WriteLine("While inserting into the db: ", E.Message);
            }
        }
    }
}
