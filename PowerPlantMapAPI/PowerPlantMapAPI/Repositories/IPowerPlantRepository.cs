using PowerPlantMapAPI.Data.Dto;

namespace PowerPlantMapAPI.Repositories;

public interface IPowerPlantRepository
{
    Task<IEnumerable<PowerPlantDataDto?>> Get();
    Task<PowerPlantDataDto?> GetById(string id);
    Task<bool> AddGenerator(GeneratorDataDto generator);
    Task<bool> AddBlocGenerator(string blocId, string generatorId);
    Task<bool> AddBloc(BlocDataDto bloc);
    Task<bool> AddPowerPlantBloc(string powerPlantId, string blocId);
    Task<bool> AddPowerPlant(PowerPlantDataDto powerPlantToBeAdded);
    Task<bool> DeletePowerPlant(string id);
}