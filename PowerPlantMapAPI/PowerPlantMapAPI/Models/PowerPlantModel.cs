namespace PowerPlantMapAPI.Models
{
    public class PowerPlantModel
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public List<ReactorModel> reactors { get; set; }
    }
}
