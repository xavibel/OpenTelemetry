using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace WebPage.Diagnostics
{
    
    public class WebPageDiagnostics
    {
        private readonly ILogger _logger;
        private static readonly ActivitySource activitySourceUsers = new("UsersModule", version: "ver1.0");

        public WebPageDiagnostics(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("WebPage");
        }
        
        public Activity UserIndex(string data)
        {
            _usersIndex(_logger, data, null);

            return activitySourceUsers.StartActivity("Users", ActivityKind.Producer);
        }

        private readonly Action<ILogger, string, Exception> _usersIndex = LoggerMessage.Define<string>(
            LogLevel.Warning, WebPageClientIds.UsersIndexEventId,
            "We are on Users Index with data {data}");
    }
}
