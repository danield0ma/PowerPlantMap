namespace PowerPlantMapAPI.Models.DTO
{
    public class PowerPlantDetailsDto
    {
        public string? BlocId { get; set; }
        public string? BlocType { get; set; }
        public int MaxBlocCapacity { get; set; }
        public int CommissionDate { get; set; }
        public string? GeneratorId { get; set; }
        public int MaxCapacity { get; set; }
    }
}
