using PowerPlantMapAPI.Data;

namespace PowerPlantMapAPI.Repositories;

public interface IEmailSubscriptionsRepository
{
    List<EmailSubscriptionModel>? Get();
    EmailSubscriptionModel? GetById(Guid id);
    EmailSubscriptionModel? GetByEmail(string email);
    void Add(EmailSubscriptionModel newSubscription);
    void Update(Guid id, string newEmail);
    void Delete(EmailSubscriptionModel emailToDelete);
}