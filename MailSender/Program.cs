using System.Diagnostics;
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
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;  
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddSingleton(new ServiceBusClient(hostContext.Configuration["ServiceBus:ConnectionString"]))
                        .AddSingleton<EventReceiver>()
                        .AddHostedService<Worker>()
                        .AddOpenTelemetryTracing(builder =>
                        {
                            builder
                                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MailSender", serviceVersion: "ver1.0"))
                                .AddSource(nameof(EventReceiver))
                                .AddGrpcCoreInstrumentation()
                                .AddConsoleExporter()
                                .AddJaegerExporter(options =>
                                {
                                    options.AgentHost = hostContext.Configuration["Jaeger:AgentHost"];
                                });
                        });
                });
    }
}