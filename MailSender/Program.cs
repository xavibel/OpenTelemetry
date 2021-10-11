using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace MailSender
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(new ServiceBusClient(hostContext.Configuration["ServiceBus:ConnectionString"]));
                    services.AddSingleton<EventReceiver>();
                    services.AddHostedService<Worker>();
                    services.AddOpenTelemetryTracing(builder =>
                    {
                        builder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MailSender"))
                            .AddJaegerExporter(options =>
                            {
                                options.AgentHost = hostContext.Configuration["Jaeger:AgentHost"];
                            });
                    });
                });
    }
}
