using Dapper;
using PowerPlantMapAPI.Models;
using System.Data;
using System.Data.SqlClient;

namespace PowerPlantMapAPI.Repositories
{
    public class PowerRepository : IPowerRepository
    {
        private readonly SqlConnection _connection;

        public PowerRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("Local"));
            _connection.Open();
        }

        public async Task<List<PowerPlantDataModel>> QueryPowerPlantBasics()
        {
            return (List<PowerPlantDataModel>)await
                _connection.QueryAsync<PowerPlantDataModel>("[PowerPlantBasics]", CommandType.StoredProcedure);
        }

        public async Task<PowerPlantDataModel> QueryBasicsOfPowerPlant(string id)
        {
            var parameters = new { id = id };
            List<PowerPlantDataModel> PP = (List<PowerPlantDataModel>) await _connection.QueryAsync<PowerPlantDataModel>
                ("[GetBasicsOfPowerPlant]", parameters, commandType: CommandType.StoredProcedure);
            return PP[0];
        }

        public async Task<List<PowerPlantDetailsModel>> QueryPowerPlantDetails(string id)
        {
            var parameters = new { PowerPlantID = id };
            return (List<PowerPlantDetailsModel>)await _connection.
                QueryAsync<PowerPlantDetailsModel>("GetPowerPlantDetails",
                parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<List<PastActivityModel>> QueryPastActivity(string generator, DateTime start, DateTime end)
        {
            var parameters = new { GID = generator, start = start, end = end.AddMinutes(15) };
            List<PastActivityModel> PastActivity = (List<PastActivityModel>)await _connection.QueryAsync<PastActivityModel>
                ("GetPastActivity", parameters, commandType: CommandType.StoredProcedure);
            return PastActivity;
        }

        public async Task<List<string>> QueryPowerPlants()
        {
            List<string> PowerPlants = (List<string>)await _connection.QueryAsync<string>
                    ("GetPowerPlants", commandType: CommandType.StoredProcedure);
            return PowerPlants;
        }

        public async Task<List<string>> QueryGeneratorsOfPowerPlant(string PowerPlant)
        {
            var parameters = new { PPID = PowerPlant };
            List<string> Generators = (List<string>)await _connection.QueryAsync<string>
                ("GetGeneratorsOfPowerPlant", parameters, commandType: CommandType.StoredProcedure);
            return Generators;
        }

        public async Task<List<DateTime>> QueryLastDataTime()
        {
            var parameter = new { PPID = "PKS" };
            List<DateTime> LastData = (List<DateTime>)await _connection.QueryAsync<DateTime>
                ("GetLastDataTime", parameter, commandType: CommandType.StoredProcedure);
            return LastData;
        }

        public async Task<List<string>> QueryGenerators()
        {
            List<string> generators = (List<string>)await _connection.QueryAsync<string>
                ("GetGenerators", commandType: CommandType.StoredProcedure);
            return generators;
        }

        public async Task InsertData(string GID, DateTime start, int power)
        {
            var par = new { GID = GID, start = start, end = start.AddMinutes(15), power = power };
            try
            {
                await _connection.QueryAsync("AddPastActivity", par, commandType: CommandType.StoredProcedure);
            }
            catch (Exception E)
            {
                System.Diagnostics.Debug.WriteLine(E.Message);
                //TODO SQL UPDATE COMMAND kellene!!
            }
        }
    }
}
