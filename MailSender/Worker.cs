using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace MailSender
{
    public class Worker : BackgroundService
    {
        private readonly ServiceBusClient _client;
        private readonly EventReceiver _eventReceiver;
        private readonly IConfiguration _configuration;
        private readonly ILogger<Worker> _logger;
        private ServiceBusProcessor _processor;

        public Worker(ServiceBusClient client, EventReceiver eventReceiver, IConfiguration configuration, ILogger<Worker> logger)
        {
            _client = client;
            _eventReceiver = eventReceiver;
            _configuration = configuration;
            _logger = logger;
        }
        
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        public override Task StartAsync(CancellationToken stoppingToken)
        {

            _processor = _client.CreateProcessor(_configuration["ServiceBus:QueueName"], new ServiceBusProcessorOptions());

            _processor.ProcessMessageAsync += _eventReceiver.MessageHandler;
            _processor.ProcessErrorAsync += _eventReceiver.ErrorHandler;

            _processor.StartProcessingAsync(stoppingToken);

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Starting message processor");
                _processor.StopProcessingAsync(stoppingToken);
            }
            finally
            {
                _processor.DisposeAsync();
                _client.DisposeAsync();
            }
            return Task.CompletedTask;
        }
    }
}
