namespace PowerPlantMapAPI.Models
{
    public class PowerPlantDetailsModel
    {
        public string? PowerPlantID { get; set; }
        public string? name { get; set; }
        public string description { get; set; }
        public string? OperatorCompany { get; set; }
        public string? webpage { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public int CurrentPower { get; set; }
        public int MaxPower { get; set; }
        public DateTime DataStart { get; set; }
        public DateTime DataEnd { get; set; }
        public string? Color { get; set; }
        public string? Address { get; set; }
        public bool IsCountry { get; set; }
        public List<BlocModel> Blocs { get; set; }
    }
}
