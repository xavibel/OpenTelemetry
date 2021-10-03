using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Messages;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebPage.Diagnostics;
using WebPage.Models;

namespace WebPage.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly WebPageDiagnostics _diagnostics;

        public HomeController(IHttpClientFactory httpClientFactory, WebPageDiagnostics diagnostics)
        {
            _diagnostics = diagnostics;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            using (var activity = _diagnostics.HomeIndex("Extra data"))
            {
                activity.AddTag("tag1", "value");
                activity.AddEvent(new ActivityEvent("my event"));
                activity.AddBaggage("bag1", "value bag 1"); 

                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync("http://localhost:5000/weatherforecast");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> WeatherAsync()
        {
            var rng = new Random();
            var weather = new WeatherForecast
                {
                    Date = DateTime.Now,
                    TemperatureC = rng.Next(-20, 55),
                    Summary = WeatherForecast.Summaries[rng.Next(WeatherForecast.Summaries.Length)]
                };
            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(weather), Encoding.UTF8);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.PostAsync("http://localhost:5000/weatherforecast", httpContent);
            return View("Index");
        }
    }
}
