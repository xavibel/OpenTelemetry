using System.Diagnostics;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace MyService.Services
{
    public class ServiceBus
    {
        private readonly ServiceBusClient _client;
        private static readonly ActivitySource ActivitySource = new ActivitySource(nameof(ServiceBus), version: "ver1.0");

        public ServiceBus(ServiceBusClient client)
        {
            _client = client;
        }

        public ServiceBusSender CreateSender(string queueOrTopicName)
        {
            using (var activity = ActivitySource.StartActivity(SetActivityName("Create Sender"), ActivityKind.Producer))
            {
                activity.AddEvent(new ActivityEvent("Creating sender"));
                activity.AddTag("queueOrTopic.name", queueOrTopicName);

                return _client.CreateSender(queueOrTopicName);
            }
        }

        public async Task SendMessage(ServiceBusSender sender, ServiceBusMessage message)
        {
            using (var activity = ActivitySource.StartActivity(SetActivityName("Send message"), ActivityKind.Producer))
            {
                activity.AddEvent(new ActivityEvent("Sending message"));
                activity.AddTag("sender", sender.FullyQualifiedNamespace);

                await sender.SendMessageAsync(message);
            }
        }

        private static string SetActivityName(string activityName)
        {
            return $"{ActivitySource.Name}: {activityName}";
        }
    }
}