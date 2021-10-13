using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
            _eventReceiver.StartAsync(stoppingToken);
            return Task.CompletedTask;
        }
    }
}
