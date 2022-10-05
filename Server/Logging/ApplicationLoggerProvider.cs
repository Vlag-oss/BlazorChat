using BlazorChat.Server.Models;

namespace BlazorChat.Server.Logging
{
    public class ApplicationLoggerProvider : ILoggerProvider
    {
        private readonly BlazorChatContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationLoggerProvider(BlazorChatContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new DatabaseLogger(_context, _httpContextAccessor);
        }
    }
}
