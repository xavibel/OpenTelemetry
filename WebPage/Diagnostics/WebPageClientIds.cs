using Microsoft.Extensions.Logging;

namespace WebPage.Diagnostics
{
    public class WebPageClientIds
    {
        public static EventId HomeIndexEventId = new EventId(100, nameof(HomeIndexEventId));
        public static EventId UsersIndexEventId = new EventId(200, nameof(UsersIndexEventId));
    }
}