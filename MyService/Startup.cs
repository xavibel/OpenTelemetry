using System;
using System.Collections.Generic;
using System.Diagnostics;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyService.Data;
using MyService.Events;
using MyService.Services;
using IApplicationLifetime = Microsoft.AspNetCore.Hosting.IApplicationLifetime;

namespace MyService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSingleton(new ServiceBus(new ServiceBusClient(Configuration["ServiceBus:ConnectionString"])));
            services.AddTransient<UserCreated>();
            ConfigureOpenTelemetry(services);
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo {Title = "MyService", Version = "v1"});
            });
            services.AddEntityFrameworkNpgsql()
                .AddDbContext<MyServiceContext>(options => options.UseNpgsql(Configuration.GetConnectionString("MyServiceContext")));
        }

        private void ConfigureOpenTelemetry(IServiceCollection services)
        {
            services.AddOpenTelemetryTracing(builder =>
            {
                builder.SetResourceBuilder(ResourceBuilder
                        .CreateDefault()
                        .AddService("MyService", serviceVersion: "ver1.0"))
                    .AddSource("ServiceBus")
                    .AddSource("UserCreated")
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
                    .AddSqlClientInstrumentation(options => options.SetDbStatementForText = true)
                    .AddConsoleExporter()
                    .AddJaegerExporter(options => { options.AgentHost = Configuration["Jaeger:AgentHost"]; });
                
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory factory, IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyService v1"));
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            IDisposable innerSubscription = null;
            IDisposable outerSubscription = DiagnosticListener.AllListeners.Subscribe(delegate (DiagnosticListener listener)
            {
                // subscribe to the Service Bus DiagnosticSource
                if (listener.Name == "Azure.Messaging.ServiceBus")
                {
                    // receive event from Service Bus DiagnosticSource
                    innerSubscription = listener.Subscribe(delegate (KeyValuePair<string, object> busEvent)
                    {
                        if (busEvent.Key.EndsWith("Stop"))
                        {
                            using var currentActivity = Activity.Current;
                            currentActivity?.Parent?.AddEvent(new ActivityEvent(
                                name: $"{currentActivity.DisplayName}", 
                                timestamp: currentActivity.StartTimeUtc, 
                                tags: new ActivityTagsCollection(new List<KeyValuePair<string, object?>>
                                {
                                    new("operation.id", currentActivity.Id),
                                    new("operation.name", currentActivity.OperationName),
                                    new("operation.start", currentActivity.StartTimeUtc),
                                    new("duration", currentActivity.Duration)
                                })));
                        }
                    });
                }
            });

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                outerSubscription?.Dispose();
                innerSubscription?.Dispose();
            });
        }
    }
}
