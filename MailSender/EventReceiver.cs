using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace MailSender
{
    public class EventReceiver
    {
        private static readonly ActivitySource ActivitySource = new(nameof(EventReceiver));
        private static readonly TextMapPropagator Propagator = new TraceContextPropagator();
        
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
                }
                await processor.StopProcessingAsync(stoppingToken);
            }
            finally
            {
                await processor.DisposeAsync();
                await _client.DisposeAsync();
            }

        }

        static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            var msg = args.Message;
            var activity = StartActivity(msg);

            string body = args.Message.Body.ToString();
            activity.SetTag("producer.message", body);

            await args.CompleteMessageAsync(args.Message);
        }

        private static Activity StartActivity(ServiceBusReceivedMessage msg)
        {
            var parentContext = Propagator.Extract(default, msg.ApplicationProperties, ExtractTraceParent);

            Baggage.Current = parentContext.Baggage;

            using var activity =
                ActivitySource.StartActivity("Receive message", ActivityKind.Consumer, parentContext.ActivityContext);
            return activity;
        }

        private static IEnumerable<string> ExtractTraceParent(IReadOnlyDictionary<string, object> props, string key)
        {
            var traceProperties = new List<string>();
            if (props.TryGetValue(key, out var value))
            {
                traceProperties.Add(value.ToString());
            }

            return traceProperties;
        }


        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            //TODO: Log and trace error
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
