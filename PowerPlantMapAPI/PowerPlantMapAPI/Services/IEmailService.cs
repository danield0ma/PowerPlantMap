namespace PowerPlantMapAPI.Services
{
    public interface IEmailService
    {
        public string SendEmail(string? to, string? subject, string? body);
    }
}