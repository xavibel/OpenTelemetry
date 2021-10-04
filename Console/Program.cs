using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace Console
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var client = new ServiceBusClient(configuration["ServiceBus:ConnectionString"]);

            var receiver = client.CreateReceiver("SC2021");
            var message = await receiver.ReceiveMessageAsync();
            
            System.Console.WriteLine(message.Body.ToString());
        }
    }
}
