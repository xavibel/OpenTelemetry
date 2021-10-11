using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Graylog;
using Serilog.Sinks.Graylog.Core.Transport;

namespace MyService
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
                .WriteTo.Console(LogEventLevel.Information)
                .WriteTo.Graylog(new GraylogSinkOptions
                {
                    MinimumLogEventLevel = LogEventLevel.Information,
                    HostnameOrAddress = "127.0.0.1",
                    Port = 12201,
                    TransportType = TransportType.Udp
                })
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "MyService")
                .CreateLogger();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.AddSerilog();
                });
    }
}
