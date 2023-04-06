using Dapper;
using Microsoft.Extensions.Configuration;
using PowerPlantMapAPI.Models;
using PowerPlantMapAPI.Models.DTO;
using System.Data;
using System.Data.SqlClient;

namespace PowerPlantMapAPI.Repositories
{
    public class PowerRepository : IPowerRepository
    {
        private readonly SqlConnection _connection;

        public PowerRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            _connection.Open();
        }

        public async Task<List<PowerPlantDataDTO>> QueryPowerPlantBasics()
        {
            List<PowerPlantDataDTO> PowerPlants = (List<PowerPlantDataDTO>) await
                _connection.QueryAsync<PowerPlantDataDTO>("[PowerPlantBasics]", CommandType.StoredProcedure);
            return PowerPlants;
        }

        public async Task<List<PowerPlantDetailsDTO>> QueryPowerPlantDetails(string id)
        {
            var parameters = new { PowerPlantID = id };
            List<PowerPlantDetailsDTO> PowerPlantDetails =
                (List<PowerPlantDetailsDTO>)await _connection.
                QueryAsync<PowerPlantDetailsDTO>("GetPowerPlantDetails",
                    parameters, commandType: CommandType.StoredProcedure);
            return PowerPlantDetails;
        }

        public async Task<List<PowerPlantDataDTO>> QueryBasicsOfPowerPlant(string id)
        {
            var parameters = new { id = id };
            List<PowerPlantDataDTO> PP = (List<PowerPlantDataDTO>) await
                _connection.QueryAsync<PowerPlantDataDTO>
                ("[GetBasicsOfPowerPlant]", parameters, commandType: CommandType.StoredProcedure);
            return PP;
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
    }
}
