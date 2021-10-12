using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace MailSender
{
    internal class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new ServiceBusClient(Configuration["ServiceBus:ConnectionString"]));
            services.AddOpenTelemetryTracing(builder => builder
                .SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService("MailSender", serviceVersion: "ver1.0"))
                .AddAspNetCoreInstrumentation()
                .AddSource(nameof(EventReceiver))
                .AddConsoleExporter()
                .AddJaegerExporter(options =>
                {
                    options.AgentHost = Configuration["Jaeger:AgentHost"];
                })
            );

            services.AddHostedService<Worker>();
            services.AddTransient<EventReceiver>();
        }

        public void Configure(IApplicationBuilder app)
        {
        }
    }
}