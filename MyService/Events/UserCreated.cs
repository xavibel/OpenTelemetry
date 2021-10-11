using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using MyService.Models;
using Newtonsoft.Json;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace MyService.Events
{
    public class UserCreated
    {
        private static readonly ActivitySource ActivitySource = new(nameof(UserCreated), "ver1.0");
        private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
        private readonly ServiceBusClient _client;

        public UserCreated(ServiceBusClient client)
        {
            _client = client;
        }

        public async Task CreatedUserEvent(User user)
        {
            using var activity = ActivitySource.StartActivity("Send message", ActivityKind.Producer);

            var sender = _client.CreateSender("SC2021");
            var createdUser = new Messages.User()
            {
                Name = user.Name,
                LastName = user.LastName,
                MailAddress = user.MailAddress,
                BirthDate = user.BirthDate
            };

            var msg = new ServiceBusMessage(new BinaryData(JsonConvert.SerializeObject(createdUser)));

            Propagator.Inject(new PropagationContext(activity.Context, Baggage.Current), msg.ApplicationProperties,
                (props, key, value) => props[key] = value);
            activity.SetTag("producer.message", JsonConvert.SerializeObject(createdUser));

            await sender.SendMessageAsync(msg);
        }
    }
}