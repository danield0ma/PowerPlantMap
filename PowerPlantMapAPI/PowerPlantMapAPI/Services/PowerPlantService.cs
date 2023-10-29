using System.Data;
using Microsoft.AspNetCore.Mvc;
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
        var allDataOfPowerPlants = await _powerPlantRepository.GetPowerPlantsModel();
        if (allDataOfPowerPlants.FirstOrDefault() is null) throw new ArgumentException("PowerPlant not found!");
        
        foreach (var allDataOfPowerPlant in allDataOfPowerPlants.
            GroupBy(x => x.PowerPlantId).
            Select(group => group.First()).ToList())
        {
            var powerPlant = MapToPowerPlantModel(allDataOfPowerPlant);
            var currentPowerPlants = allDataOfPowerPlants.
                Where(y => y.PowerPlantId == allDataOfPowerPlant.PowerPlantId).ToList();
            foreach (var currentPowerPlant in currentPowerPlants)
            {
                var blocsOfPowerPlant = new List<BlocDataDto>();
                foreach (var currentBloc in currentPowerPlants.
                    GroupBy(z => z.BlocId ).Select(group => group.First()).ToList())
                {
                    var blocData = MapToBlocDataDto(allDataOfPowerPlant);
                    List<GeneratorDataDto> generatorsOfBloc = new();
                    foreach (var item in currentPowerPlants.
                        Where(x => x.BlocId == currentBloc.BlocId).ToList())
                    {
                        GeneratorDataDto generatorDetails = new()
                        {
                            GeneratorId = item.GeneratorId,
                            MaxCapacity = item.MaxCapacity,
                        };
                        generatorsOfBloc.Add(generatorDetails);
                    }
                    blocData.Generators = generatorsOfBloc;
                    blocsOfPowerPlant.Add(blocData);
                }
                powerPlant.Blocs = blocsOfPowerPlant;
            }
            powerPlants.Add(powerPlant);
        }
        return powerPlants;
    }
    
    public async Task<PowerPlantModel?> GetById(string id)
    {
        var allDataOfPowerPlant = await _powerPlantRepository.GetPowerPlantModel(id);
        if (allDataOfPowerPlant.FirstOrDefault() is null) throw new ArgumentException("PowerPlant not found!");
        
        var powerPlant = MapToPowerPlantModel(allDataOfPowerPlant.FirstOrDefault()!);
        
        List<BlocDataDto> blocsOfPowerPlant = new();
        foreach (var powerPlantDetail in allDataOfPowerPlant.
                     GroupBy(x => x.BlocId ).
                     Select(group => group.First()).ToList())
        {
            var blocData = MapToBlocDataDto(allDataOfPowerPlant.FirstOrDefault()!);
            List<GeneratorDataDto> generatorsOfBloc = new();
            foreach (var item in allDataOfPowerPlant.
                Where(x => x.BlocId == powerPlantDetail.BlocId).ToList())
            {
                GeneratorDataDto generatorDetails = new()
                {
                    GeneratorId = item.GeneratorId,
                    MaxCapacity = item.MaxCapacity,
                };
                generatorsOfBloc.Add(generatorDetails);
            }
            blocData.Generators = generatorsOfBloc;
            blocsOfPowerPlant.Add(blocData);
        }
        powerPlant.Blocs = blocsOfPowerPlant;
        
        return powerPlant;
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
        var powerPlantDetails = await _powerPlantRepository.GetPowerPlantDetails(id);
        var generatorsOfPowerPlant = powerPlantDetails.Select(x=>x.GeneratorId).ToList();
        var blocsOfPowerPlant = powerPlantDetails.Select(x=>x.BlocId).ToList();
        
        foreach (var generator in generatorsOfPowerPlant)
        {
            await _powerPlantRepository.DeleteGenerator(generator!);
        }
        
        foreach (var bloc in blocsOfPowerPlant)
        {
            await _powerPlantRepository.DeleteBloc(bloc!);
        }
        
        return await _powerPlantRepository.DeletePowerPlant(id);
    }

    private PowerPlantModel MapToPowerPlantModel(AllDataOfPowerPlantDto? allDataOfPowerPlant)
    {
        if (allDataOfPowerPlant is null) return new PowerPlantModel();
        return new PowerPlantModel
        {
            PowerPlantId = allDataOfPowerPlant.PowerPlantId,
            Name = allDataOfPowerPlant.Name,
            Description = allDataOfPowerPlant.Description,
            OperatorCompany = allDataOfPowerPlant.OperatorCompany,
            Webpage = allDataOfPowerPlant.Webpage,
            Image = allDataOfPowerPlant.Image,
            Longitude = allDataOfPowerPlant.Longitude,
            Latitude = allDataOfPowerPlant.Latitude,
            Color = allDataOfPowerPlant.Color,
            Address = allDataOfPowerPlant.Address,
            IsCountry = allDataOfPowerPlant.IsCountry
        };
    }
    
    private BlocDataDto MapToBlocDataDto(AllDataOfPowerPlantDto? bloc)
    {
        if (bloc is null) return new BlocDataDto();
        return new BlocDataDto()
        {
            BlocId = bloc.BlocId,
            BlocType = bloc.BlocType,
            MaxBlocCapacity = bloc.MaxBlocCapacity,
            CommissionDate = bloc.CommissionDate
        };
    }
}