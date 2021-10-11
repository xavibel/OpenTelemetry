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
using MyService.Data;
using MyService.Events;

namespace MyService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSingleton(new ServiceBusClient(Configuration["ServiceBus:ConnectionString"]));
            services.AddTransient<UserCreated>();
            services.AddOpenTelemetryTracing(builder =>
            { 
                builder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MyService", serviceVersion: "ver1.0"))
                    .AddSource(nameof(UserCreated))
                    .AddAspNetCoreInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
                    .AddSqlClientInstrumentation(options => options.SetDbStatementForText = true)
                    .AddConsoleExporter()
                    .AddJaegerExporter(options =>
                    {
                        options.AgentHost = Configuration["Jaeger:AgentHost"]; 
                    });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyService", Version = "v1" });
            });
            services.AddEntityFrameworkNpgsql().AddDbContext<MyServiceContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("MyServiceContext")));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyService v1"));
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
