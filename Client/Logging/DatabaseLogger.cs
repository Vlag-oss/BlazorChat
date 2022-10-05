using System.Globalization;
using System.Net.Http.Json;
using System.Security.Claims;
using BlazorChat.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorChat.Client.Logging;

public class DatabaseLogger : ILogger
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public DatabaseLogger(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider)
    {
        _httpClient = httpClient;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Task.Run(async () =>
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var userId = authState.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            var log = new Log
            {
                LogLevel = logLevel.ToString(),
                EventName = eventId.Name,
                ExceptionMessage = exception?.Message,
                StackTrace = exception?.StackTrace,
                Source = "Client",
                CreatedDate = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                UserId = Convert.ToInt64(userId)
            };

            await _httpClient.PostAsJsonAsync("/logs", log);
        });
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null!;
    }
}