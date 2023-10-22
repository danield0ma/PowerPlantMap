using System.Globalization;
using PowerPlantMapAPI.Helpers;
using PowerPlantMapAPI.Repositories;
using PowerPlantMapAPI.Services;

namespace PowerPlantMapAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy//.AllowCredentials()
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowAnyOrigin();
                    });
            });

            builder.Services.AddScoped<IPowerService, PowerService>();
            builder.Services.AddScoped<IDateHelper, DateHelper>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IStatisticsService, StatisticsService>();
            builder.Services.AddScoped<IPowerRepository, PowerRepository>();
            builder.Services.AddScoped<IPowerHelper, PowerHelper>();
            builder.Services.AddScoped<IXmlHelper, XmlHelper>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            var customCulture = new CultureInfo("hu-HU")
            {
                NumberFormat =
                {
                    NumberGroupSeparator = " ",
                    NumberDecimalDigits = 3,
                    NumberDecimalSeparator = ","
                }
            };
            CultureInfo.DefaultThreadCurrentCulture = customCulture;
            CultureInfo.DefaultThreadCurrentUICulture = customCulture;

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}