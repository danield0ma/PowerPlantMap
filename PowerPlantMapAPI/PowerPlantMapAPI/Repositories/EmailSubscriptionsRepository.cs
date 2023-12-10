using PowerPlantMapAPI.Data;

namespace PowerPlantMapAPI.Repositories;

public class EmailSubscriptionsRepository : IEmailSubscriptionsRepository
{
    private readonly ApplicationDbContext _context;

    public EmailSubscriptionsRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public List<EmailSubscriptionModel>? Get()
    {
        return _context.EmailSubscriptions?.ToList();
    }
    
    public EmailSubscriptionModel? GetById(Guid id)
    {
        return _context.EmailSubscriptions?.Find(id);
    }
    
    public EmailSubscriptionModel? GetByEmail(string email)
    {
        return _context.EmailSubscriptions?
            .Where(x => x.Email == email).FirstOrDefault();
    }

    public void Add(EmailSubscriptionModel newSubscription)
    {
        newSubscription.Id = Guid.NewGuid();
        _context.EmailSubscriptions?.Add(newSubscription);
        _context.SaveChanges();
    }
    
    public void Update(Guid id, string? newEmail)
    {
        var entry = _context.EmailSubscriptions?.Find(id);
        entry!.Email = newEmail;
        _context.EmailSubscriptions?.Update(entry);
        _context.SaveChanges();
    }
    
    public void Delete(EmailSubscriptionModel emailToDelete)
    {
        _context.EmailSubscriptions?.Remove(emailToDelete);
        _context.SaveChanges();
    }
}