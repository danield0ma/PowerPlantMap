using System.ComponentModel.DataAnnotations;

namespace PowerPlantMapAPI.Data.Dto;

public class RegistrationModel
{
    [Required]
    public string? UserName { get; set; }
    [Required]
    public string? Email { get; set; }
    [Required]
    public string? Password { get; set; }
}