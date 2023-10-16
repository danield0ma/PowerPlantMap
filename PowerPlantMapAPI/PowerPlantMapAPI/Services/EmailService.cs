using RestSharp;

namespace PowerPlantMapAPI.Services
{
    public class EmailService: IEmailService
    {
        private readonly IConfiguration _configuration;
    
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    
        public string SendEmail(string to, string subject, string body)
        {
            var client = new RestClient($"https://api.eu.mailgun.net/v3/{_configuration["Email:MailgunDomain"]}");
            var request = new RestRequest("messages", Method.Post);
            request.AddHeader("Authorization", $"Basic {Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"api:{_configuration["Email:MailgunApiKey"]}"))}");
            request.AddParameter("from", "PowerPlantMap <noreply@powerplantmap.tech>");
            request.AddParameter("to", to);
            request.AddParameter("subject", subject);
            request.AddParameter("text", body);
    
            var response = client.Execute(request);
    
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("Email sent successfully!");
                return "Email sent successfully!";
            }
            else
            {
                Console.WriteLine($"Error sending email: {response.ErrorMessage}");
                return "Error";
            }
        }
    }
}
