using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace WebPage.Diagnostics
{
    
    public class WebPageDiagnostics
    {
        private readonly ILogger _logger;
        private static ActivitySource activitySource = new ActivitySource("HomeModule", version: "ver1.0");

        public WebPageDiagnostics(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("WebPage");
        }

        public Activity HomeIndex(string data)
        {
            _homeIndex(_logger, data, null);

            return activitySource.StartActivity("Home");
        }

        private Action<ILogger, string, Exception> _homeIndex = LoggerMessage.Define<string>(
            LogLevel.Warning, WebPageClientIds.HomeIndexEventId,
            "We are on Home Index with data {data}");
    }

    public class WebPageClientIds
    {
        public static EventId HomeIndexEventId = new EventId(100, nameof(HomeIndexEventId));
    }
}
