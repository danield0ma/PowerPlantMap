namespace PowerPlantMapAPI.Models.DTO
{
    public class BasicsOfPowerPlantDTO
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public string? OperatorCompany { get; set; }
        public string? webpage { get; set; }
        public int MaxPower { get; set; }
        public string? Color { get; set; }
        public string? Address { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public bool IsCountry { get; set; }

    }
}
