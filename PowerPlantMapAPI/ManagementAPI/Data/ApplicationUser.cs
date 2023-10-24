using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ManagementAPI.Data
{
    public class ApplicationUser: IdentityUser
    {
        // public int Id { get; set; }
        //
        // [Required]
        // public string? Email { get; set; } = null!;
        // public string? Password { get; set; }
        //public string? Role { get; set; }
    }
}
