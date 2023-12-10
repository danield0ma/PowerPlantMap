using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PowerPlantMapAPI.Data;

public class EmailSubscriptionModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public DateTime Created { get; set; }
    public string? Email { get; set; }
}