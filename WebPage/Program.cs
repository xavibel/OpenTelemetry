using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Graylog;
using Serilog.Sinks.Graylog.Core.Transport;
using Serilog.Sinks.Loki;

namespace WebPage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateLogger();
            CreateHostBuilder(args).Build().Run();
        }

        private static void CreateLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .CreateLogger();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog((context, configuration) =>
                {
                    configuration.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .Enrich.WithProperty("Application", "WebPage")
                        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                        .WriteTo.Console(LogEventLevel.Information)
                        .WriteTo.Graylog(new GraylogSinkOptions
                        {
                            MinimumLogEventLevel = LogEventLevel.Information,
                            HostnameOrAddress = "graylog-test.voxelgroup.net",
                            Port = 12201,
                            Facility = "training",
                            TransportType = TransportType.Udp
                        })
                        .WriteTo.LokiHttp(new NoAuthCredentials("http://docker-stag.voxelgroup.net:3100"));
                })
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.AddSerilog();
                });
    }
}
