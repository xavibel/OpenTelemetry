using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace Console
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = BuildConfiguration();

            var client = new ServiceBusClient(configuration["ServiceBus:ConnectionString"]);
            var receiver = client.CreateReceiver(configuration["ServiceBus:QueueName"]);
            var message = await receiver.ReceiveMessageAsync();
            
            System.Console.WriteLine(message.Body.ToString());
        }

        private static IConfiguration BuildConfiguration()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            return configuration;
        }
    }
}
