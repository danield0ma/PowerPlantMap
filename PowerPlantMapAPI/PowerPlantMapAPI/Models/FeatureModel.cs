using PowerPlantMapAPI.Models.DTO;

namespace PowerPlantMapAPI.Models
{
    public class FeatureModel
    {
        public string Type { get; set; }
        public FeaturePropertyModelDTO properties { get; set; }
        public FeatureGeometryModelDTO geometry { get; set; }
    }
}
