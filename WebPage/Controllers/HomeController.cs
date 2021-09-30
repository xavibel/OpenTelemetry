using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
                var response = await httpClient.GetAsync("http://localhost:1745/weatherforecast");
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
    }
}
