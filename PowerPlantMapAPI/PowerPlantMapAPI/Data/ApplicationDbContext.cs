using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PowerPlantMapAPI.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<EmailSubscriptionModel>? EmailSubscriptions { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        
    }
    
}