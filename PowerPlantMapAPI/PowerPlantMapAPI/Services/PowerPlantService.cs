using System.Data;
using PowerPlantMapAPI.Data;
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
    
    public async Task<IEnumerable<PowerPlantModel?>> Get()
    {
        List<PowerPlantModel?> powerPlants = new();
        var powerPlantNames = await _powerPlantRepository.GetPowerPlantNames();
        foreach (var powerPlantName in powerPlantNames)
        {
            powerPlants.Add(await GetById(powerPlantName));
        }
        return powerPlants;
    }
    
    public async Task<PowerPlantModel?> GetById(string id)
    {
        var powerPlant = await _powerPlantRepository.GetById(id);
        var result = new PowerPlantModel()
        {
            PowerPlantId = powerPlant?.PowerPlantId,
            Name = powerPlant?.Name,
            Description = powerPlant?.Description,
            OperatorCompany = powerPlant?.OperatorCompany,
            Webpage = powerPlant?.Webpage,
            Image = powerPlant?.Image,
            Longitude = powerPlant!.Longitude,
            Latitude = powerPlant.Latitude,
            Color = powerPlant.Color,
            Address = powerPlant.Address,
            IsCountry = powerPlant.IsCountry
        };
        
        List<BlocDataDto> blocs = new();
        var powerPlantDetails = await _powerPlantRepository.GetPowerPlantDetails(id);
        foreach (var powerPlantDetail in powerPlantDetails.
                     GroupBy(x => x.BlocId ).
                     Select(group => group.First()).ToList())
        {
            BlocDataDto blocData = new()
            {
                BlocId = powerPlantDetail.BlocId,
                BlocType = powerPlantDetail.BlocType,
                MaxBlocCapacity = powerPlantDetail.MaxBlocCapacity,
                CommissionDate = powerPlantDetail.CommissionDate
            };

            List<GeneratorDataDto> generators = new();
            foreach (var item in powerPlantDetails.
                         Where(x => x.BlocId == powerPlantDetail.BlocId).ToList())
            {
                GeneratorDataDto generatorDetails = new()
                {
                    GeneratorId = item.GeneratorId,
                    MaxCapacity = item.MaxCapacity,
                };
                generators.Add(generatorDetails);
            }
            blocData.Generators = generators;
            blocs.Add(blocData);
        }
        result.Blocs = blocs;
        
        return result;
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