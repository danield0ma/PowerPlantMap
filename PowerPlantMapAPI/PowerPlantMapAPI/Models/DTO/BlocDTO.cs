namespace PowerPlantMapAPI.Models.DTO
{
    public class BlocDTO
    {
        public string? BlocID { get; set; }
        public string? BlocType { get; set; }
        public int MaxBlocCapacity { get; set; }
        public int ComissionDate { get; set; }
        public int CurrentPower { get; set; }
        public int MaxPower { get; set; }
        public List<GeneratorDTO> Generators { get; set; }
    }
}
