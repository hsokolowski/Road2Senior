using Application;
using Azure.Identity;
using Infrastructure;
using Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Road2Senior;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

// Konfiguracja appsettings.json + Azure Key Vault
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

var keyVaultUri = builder.Configuration["KeyVaultUri"];
if (!string.IsNullOrEmpty(keyVaultUri))
{
    builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());
}

// Dodanie usług aplikacji (Application, Infrastructure, Database)
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddDatabase(builder.Configuration, builder.Environment.EnvironmentName);
//--builder.Services.RegisterDb(builder.Configuration, builder.Environment);
//--builder.Services.RegisterInfrastructure(builder.Configuration.GetSection("Infrastructure"));

// Kontrolery i Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key needed to access the endpoints. X-Api-Key: My_API_Key",
        In = ParameterLocation.Header,
        Name = "X-Api-Key",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                Scheme = "ApiKeyScheme",
                Name = "ApiKey",
                In = ParameterLocation.Header,

            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ApiKeyMiddleware>();
app.UseHttpsRedirection();

app.MapGet("/test-db", async (FootballContext db) =>
{
    try
    {
        var canConnect = await db.Database.CanConnectAsync();

        return Results.Ok(new
        {
            success = canConnect,
            server = db.Database.GetDbConnection().DataSource,
            database = db.Database.GetDbConnection().Database,
            user = Environment.UserName,
            provider = db.Database.ProviderName,
            connectionStringPreview = db.Database.GetDbConnection().ConnectionString,
            dbType = builder.Configuration["DatabaseType"]
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.ToString());
    }
});

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.MapGet("/", (IConfiguration config) => Results.Ok(new
{
    Message = "API działa poprawnie 🚀",
    StartedAt = DateTime.UtcNow,
    Environment = app.Environment.EnvironmentName,
    Build = config["BUILD_VERSION"] ?? "LOCAL",
    Endpoints = new[]
    {
        "/swagger",
        "/weatherforecast",
        "/test-db",
        "/api/externalleague/leagues",
        "/api/internalleague/league"
    }
}));

app.MapControllers();
app.Run();

namespace Road2Senior
{
    record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);
    }

    public partial class Program { }
}