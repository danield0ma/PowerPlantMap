using System.Data;
using PowerPlantMapAPI.Repositories;
using PowerPlantMapAPI.Data.Dto;

namespace PowerPlantMapAPI.Services;

public class PowerPlantService : IPowerPlantService
{
    private readonly IPowerPlantRepository _powerPlantRepository;

    public PowerPlantService(IPowerPlantRepository powerPlantRepository)
    {
        _powerPlantRepository = powerPlantRepository;
    }
    
    public async Task<IEnumerable<PowerPlantDataDto?>> Get()
    {
        return await _powerPlantRepository.Get();
    }
    
    public async Task<PowerPlantDataDto?> GetById(string id)
    {
        return await _powerPlantRepository.GetById(id);
    }
    
    public async Task<bool> AddPowerPlant(CreatePowerPlantDto powerPlantToBeAdded)
    {
        var powerPlantResult = await _powerPlantRepository.AddPowerPlant(new PowerPlantDataDto()
        {
            PowerPlantId = powerPlantToBeAdded.PowerPlantId,
            Name = powerPlantToBeAdded.Name,
            Description = powerPlantToBeAdded.Description,
            OperatorCompany = powerPlantToBeAdded.OperatorCompany,
            Webpage = powerPlantToBeAdded.Webpage,
            Image = powerPlantToBeAdded.Image,
            Longitude = powerPlantToBeAdded.Longitude,
            Latitude = powerPlantToBeAdded.Latitude,
            Color = powerPlantToBeAdded.Color,
            Address = powerPlantToBeAdded.Address,
            IsCountry = powerPlantToBeAdded.IsCountry
        });
         
        if (!powerPlantResult) 
            throw new DuplicateNameException($"PowerPlant already exists in database: {powerPlantToBeAdded.PowerPlantId}");
         
        foreach (var bloc in powerPlantToBeAdded.Blocs!)
        {
            var blocResult = await _powerPlantRepository.AddBloc(bloc);
            if (!blocResult) 
                throw new DuplicateNameException($"Bloc already exists in database: {bloc.BlocId}");
             
            foreach (var generator in bloc.Generators!)
            {
                var generatorResult = await _powerPlantRepository.AddGenerator(generator);
                if (!generatorResult)
                    throw new DuplicateNameException($"Generator already exists in database: {generator.GeneratorId}");
                
                await _powerPlantRepository.AddBlocGenerator(bloc.BlocId!, generator.GeneratorId!);
            }
            
            await _powerPlantRepository.AddPowerPlantBloc(powerPlantToBeAdded.PowerPlantId!, bloc.BlocId!);
        }
        
        return true;
    }
    
    public async Task<bool> DeletePowerPlant(string id)
    {
        return await _powerPlantRepository.DeletePowerPlant(id);
    }
}