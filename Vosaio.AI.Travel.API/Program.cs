using Microsoft.EntityFrameworkCore;
using OpenAI;
using Vosaio.AI.Travel.API.OpenAI;
using Vosaio.AI.Travel.API.Services;
using Vosaio.AI.Travel.Data;

namespace Vosaio.AI.Travel.API;

public class Program
{
    public static void Main(string[] args)
    {
        #region Builder
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<TravelContext>(op => op.UseInMemoryDatabase("TravelItineraries"));

        // Register the OpenAIClient
        builder.Services.AddSingleton<IOpenAIClient>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var openAISection = configuration.GetSection("OpenAI");

            var apiKey = openAISection["ApiKey"];
            var organizationId = openAISection["OrganizationId"];
            var projectId = openAISection["ProjectId"];

            var auth = new OpenAIAuthentication(apiKey, organizationId, projectId);
            var settings = OpenAIClientSettings.Default;
            var client = new OpenAIClient(auth, settings);

            // Return the adapter that implements IOpenAIClient
            return new OpenAIClientAdapter(client);
        });

        // Register your custom services.
        builder.Services.AddScoped<IItineraryRepository, ItineraryRepository>();
        builder.Services.AddScoped<IPromptBuilder, PromptBuilder>();
        builder.Services.AddScoped<IAIIntegrationService, AIIntegrationService>();
        builder.Services.AddScoped<IItineraryService, ItineraryService>();

        #endregion

        #region App
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();

        #endregion
    }
}
