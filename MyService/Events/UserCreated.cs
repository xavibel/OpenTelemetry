using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using MyService.Models;
using MyService.Services;
using Newtonsoft.Json;

namespace MyService.Events
{
    public class UserCreated
    {
        private readonly ServiceBus _serviceBus;
        private static readonly ActivitySource ActivitySource = new ActivitySource(nameof(UserCreated), version: "ver1.0");

        public UserCreated(ServiceBus serviceBus)
        {
            _serviceBus = serviceBus;
        }

        public async Task CreateEvent(User user)
        {
            using (var activity = ActivitySource.StartActivity("Create user", ActivityKind.Producer))
            {
                activity.AddEvent(new ActivityEvent("Create user event"));

                var sender = _serviceBus.CreateSender("SC2021");
                var createdUser = new Messages.User
                {
                    Name = user.Name,
                    LastName = user.LastName,
                    MailAddress = user.MailAddress,
                    BirthDate = user.BirthDate
                };
                var msg = new ServiceBusMessage
                {
                    Body = new BinaryData(JsonConvert.SerializeObject(createdUser))
                };

                await _serviceBus.SendMessage(sender, msg);
            }
        }
    }
}