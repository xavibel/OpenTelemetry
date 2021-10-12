﻿using System;
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
        private static readonly ActivitySource ActivitySource = new ActivitySource(nameof(EventReceiver));
        private readonly ServiceBusClient _client;
        private readonly IConfiguration _configuration;
        private static readonly TextMapPropagator _propagator = Propagators.DefaultTextMapPropagator;

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
            var parentContext = _propagator.Extract(default, msg.ApplicationProperties,
                (props, key) =>
                {
                    var traceProperties = new List<string>();
                    if (props.TryGetValue(key, out var value))
                    {
                        traceProperties.Add(value.ToString());
                    }

                    return traceProperties;
                });
            Baggage.Current = parentContext.Baggage;

            using var activity = ActivitySource.StartActivity("Receive message", ActivityKind.Consumer, parentContext.ActivityContext);
            
            string body = args.Message.Body.ToString();
            activity.SetTag("producer.message", body);

            await args.CompleteMessageAsync(args.Message);
        }

        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
