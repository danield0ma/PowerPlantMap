namespace PowerPlantMapAPI.Models
{
    public class PowerPlantDataModel
    {
        public string? PowerPlantID { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public string? OperatorCompany { get; set; }
        public string? webpage { get; set; }
        public string? image { get; set; }
        public float longitude { get; set; }
        public float latitude { get; set; }
        public string? color { get; set; }
        public string? address { get; set; }
        public bool IsCountry { get; set; }
    }
}
