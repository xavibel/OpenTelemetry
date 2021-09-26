using Microsoft.Extensions.Logging;

namespace WebPage.Diagnostics
{
    
    public class WebPageDiagnostics
    {
        private readonly ILogger _logger;

        public WebPageDiagnostics(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("WebPage");
        }

        public void HomeIndex()
        {
            LoggerMessage.Define<string>(LogLevel.Warning, WebPageClientIds.HomeIndexEventId,
                "We are on Home Index with data {data}");
        }
    }

    public class WebPageClientIds
    {
        public static EventId HomeIndexEventId = new EventId(100, nameof(HomeIndexEventId));
    }
}
