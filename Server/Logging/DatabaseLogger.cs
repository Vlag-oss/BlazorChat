using System.Globalization;
using System.Security.Claims;
using BlazorChat.Server.Models;

namespace BlazorChat.Server.Logging;

public class DatabaseLogger : ILogger
{
    private readonly BlazorChatContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DatabaseLogger(BlazorChatContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var userId = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        var log = new Log
        {
            LogLevel = logLevel.ToString(),
            EventName = eventId.Name,
            ExceptionMessage = exception?.Message,
            StackTrace = exception?.StackTrace,
            Source = "Server",
            CreatedDate = DateTime.Now.ToString(CultureInfo.InvariantCulture),
            UserId = Convert.ToInt64(userId)
        };

        _context.Add(log);
        _context.SaveChanges();
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