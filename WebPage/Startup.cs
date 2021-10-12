using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using WebPage.Diagnostics;

namespace WebPage
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
            services.AddHttpClient();
            services.AddControllersWithViews();
            services.AddSingleton<WebPageDiagnostics>();
            ConfigureOpenTelemetry(services);
        }

        private void ConfigureOpenTelemetry(IServiceCollection services)
        {
            services.AddSingleton<WebPageDiagnostics>();
            services.AddOpenTelemetryTracing(builder =>
            {
                builder.SetResourceBuilder(ResourceBuilder
                        .CreateDefault()
                        .AddService("WebPage", serviceVersion: "ver1.0"))
                    .AddSource("UsersModule")
                    .AddAspNetCoreInstrumentation(opt =>
                    {
                        opt.RecordException = true;
                    })
                    .AddHttpClientInstrumentation()
                    .AddConsoleExporter()
                    .AddJaegerExporter(options => { options.AgentHost = Configuration["Jaeger:AgentHost"]; });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Users}/{action=Index}/{id?}");
            });
        }
    }
}
