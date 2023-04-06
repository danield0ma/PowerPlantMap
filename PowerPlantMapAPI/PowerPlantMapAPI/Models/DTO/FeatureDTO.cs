namespace PowerPlantMapAPI.Models.DTO
{
    public class FeatureDTO
    {
        public string Type { get; set; } = "Feature";
        public FeaturePropertyDTO properties { get; set; }
        public FeatureGeometryDTO geometry { get; set; }
    }
}
