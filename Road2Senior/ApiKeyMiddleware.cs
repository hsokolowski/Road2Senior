using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyMiddleware> _logger;
    private const string APIKEYNAME = "X-Api-Key";

    public ApiKeyMiddleware(RequestDelegate next, ILogger<ApiKeyMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
    {
        // Sprawdzenie, czy nagłówek z kluczem API jest obecny
        if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
        {
            // Logowanie, jeśli brak klucza API w nagłówku
            _logger.LogWarning("API key was not provided in the request headers.");
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("API Key was not provided.");
            return;
        }

        // Pobranie klucza API z konfiguracji (np. Azure Key Vault)
        var apiKey = configuration.GetValue<string>("ApiKey");

        // Sprawdzenie, czy klucz API jest ustawiony w konfiguracji
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            // Logowanie błędu, jeśli klucz API nie został znaleziony w konfiguracji
            _logger.LogError("API key is missing in the configuration.");
            context.Response.StatusCode = 500; // Internal Server Error
            await context.Response.WriteAsync("Internal Server Error: API Key is missing in configuration.");
            return;
        }

        // Porównanie dostarczonego klucza z tym w konfiguracji
        if (!apiKey.Equals(extractedApiKey))
        {
            // Logowanie, jeśli klucz API nie jest zgodny
            _logger.LogWarning("Unauthorized client attempted to access with an invalid API key.");
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Unauthorized client.");
            return;
        }

        // Przejście do następnego elementu w potoku, jeśli klucz API jest prawidłowy
        await _next(context);
    }
}