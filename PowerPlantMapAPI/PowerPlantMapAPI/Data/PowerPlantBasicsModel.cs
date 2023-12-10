using PowerPlantMapAPI.Data.Dto;

namespace PowerPlantMapAPI.Data;

public class PowerPlantBasicsModel
{
    public string Type { get; set; } = "Feature";
    public FeaturePropertyDto? Properties { get; set; }
    public FeatureGeometryDto? Geometry { get; set; }
}
