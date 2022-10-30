namespace PowerPlantMapAPI.Models.DTO
{
    public class PowerPlantDataDTO
    {
        public string? PowerPlantID { get; set; }
        public string? name { get; set; }
        public string description { get; set; }
        public string? OperatorCompany { get; set; }
        public string? webpage { get; set; }
        public string? image { get; set; }
        public float longitude { get; set; }
        public float latitude { get; set; }
        public string? Color { get; set; }
        public string? Address { get; set; }
        public bool IsCountry { get; set; }
    }
}
