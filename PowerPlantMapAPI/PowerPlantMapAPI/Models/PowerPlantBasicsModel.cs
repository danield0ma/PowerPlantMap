using PowerPlantMapAPI.Models.DTO;

namespace PowerPlantMapAPI.Models
{
    public class PowerPlantBasicsModel
    {
        public string Type { get; set; } = "Feature";
        public FeaturePropertyDto? Properties { get; set; }
        public FeatureGeometryDto? Geometry { get; set; }
    }
}
