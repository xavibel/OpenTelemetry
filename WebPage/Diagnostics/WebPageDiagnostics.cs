using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace WebPage.Diagnostics
{
    
    public class WebPageDiagnostics
    {
        private readonly ILogger _logger;
        private static ActivitySource activitySource = new ActivitySource("HomeModule", version: "ver1.0");
        private static ActivitySource activitySourceUsers = new ActivitySource("UsersModule", version: "ver1.0");

        public WebPageDiagnostics(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("WebPage");
        }

        public Activity HomeIndex(string data)
        {
            _homeIndex(_logger, data, null);

            return activitySource.StartActivity("Home");
        }

        public Activity UserIndex(string data)
        {
            _usersIndex(_logger, data, null);

            return activitySourceUsers.StartActivity("Users", ActivityKind.Producer);
        }

        private Action<ILogger, string, Exception> _homeIndex = LoggerMessage.Define<string>(
            LogLevel.Warning, WebPageClientIds.HomeIndexEventId,
            "We are on Home Index with data {data}");

        private Action<ILogger, string, Exception> _usersIndex = LoggerMessage.Define<string>(
            LogLevel.Warning, WebPageClientIds.UsersIndexEventId,
            "We are on Users Index with data {data}");
    }

    public class WebPageClientIds
    {
        public static EventId HomeIndexEventId = new EventId(100, nameof(HomeIndexEventId));
        public static EventId UsersIndexEventId = new EventId(200, nameof(UsersIndexEventId));
    }
}
