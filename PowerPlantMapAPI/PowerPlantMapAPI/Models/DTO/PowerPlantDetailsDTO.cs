namespace PowerPlantMapAPI.Models.DTO
{
    public class PowerPlantDetailsDTO
    {
        public string? BlocId { get; set; }
        public string? BlocType { get; set; }
        public int MaxBlocCapacity { get; set; }
        public int ComissionDate { get; set; }
        public string GeneratorID { get; set; }
        public int MaxCapacity { get; set; }
    }
}
