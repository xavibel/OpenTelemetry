using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MailSender
{
    public class Worker : BackgroundService
    {
        private readonly EventReceiver _eventReceiver;
        private readonly ILogger<Worker> _logger;

        public Worker(EventReceiver eventReceiver, ILogger<Worker> logger)
        {
            _eventReceiver = eventReceiver;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //    await Task.Delay(1000, stoppingToken);
            //}

            _eventReceiver.StartAsync(stoppingToken);
            return Task.CompletedTask;
        }
    }
}
