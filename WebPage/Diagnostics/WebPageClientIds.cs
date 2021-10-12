using Microsoft.Extensions.Logging;

namespace WebPage.Diagnostics
{
    public class WebPageClientIds
    {
        public static EventId UsersIndexEventId = new(100, nameof(UsersIndexEventId));
    }
}