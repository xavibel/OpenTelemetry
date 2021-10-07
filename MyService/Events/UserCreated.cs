using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using MyService.Models;
using Newtonsoft.Json;

namespace MyService.Events
{
    public class UserCreated
    {
        private readonly ServiceBusClient _client;

        public UserCreated(ServiceBusClient client)
        {
            _client = client;
        }

        public async Task CreatedUserEvent(User user)
        {
            var sender = _client.CreateSender("SC2021");
            var createdUser = new Messages.User()
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

            await sender.SendMessageAsync(msg);
        }
    }
}