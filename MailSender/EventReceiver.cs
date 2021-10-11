using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace MailSender
{
    public class EventReceiver
    {
        private static readonly ActivitySource ActivitySource = new(nameof(EventReceiver), "ver1.0");
        private readonly ServiceBusClient _client;
        private readonly IConfiguration _configuration;

        public EventReceiver(ServiceBusClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }


        public async void StartAsync(CancellationToken stoppingToken)
        {
            ServiceBusProcessor processor = _client.CreateProcessor(_configuration["ServiceBus:QueueName"], new ServiceBusProcessorOptions());
            
                try
                {
                    processor.ProcessMessageAsync += MessageHandler;
                    processor.ProcessErrorAsync += ErrorHandler;

                    await processor.StartProcessingAsync(stoppingToken);
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        await processor.StopProcessingAsync();
                }
            }
                finally
                {
                    await processor.DisposeAsync();
                    await _client.DisposeAsync();
                }
           
        }

        static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            using var activity = ActivitySource.StartActivity("Receive message", ActivityKind.Consumer);
            string body = args.Message.Body.ToString();
            activity.SetTag("producer.message", body);

            await args.CompleteMessageAsync(args.Message);
        }

        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            System.Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
