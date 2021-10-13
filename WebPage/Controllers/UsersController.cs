using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebPage.Diagnostics;
using WebPage.Models;

namespace WebPage.Controllers
{
    public class UsersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly WebPageDiagnostics _diagnostics;
        private const string UsersRoute = "http://localhost:5000/api/Users";

        public UsersController(IHttpClientFactory httpClientFactory, WebPageDiagnostics diagnostics)
        {
            _httpClientFactory = httpClientFactory;
            _diagnostics = diagnostics;
        }

        // GET: UsersController
        public async Task<ActionResult> Index()
        {
            object? model;
            using (var activity = _diagnostics.UserIndex("Extra data"))
            {
                activity.AddTag("tag1", "value");
                activity.AddEvent(new ActivityEvent("my event"));
                activity.AddBaggage("bag1", "value bag 1");

                var httpClient = _httpClientFactory.CreateClient();
                var response = httpClient.GetAsync(UsersRoute);
                model = await response.Result.Content.ReadFromJsonAsync(typeof(List<User>));
            }

            return View(model);
        }

        public async Task<ActionResult> Details(Guid id)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = httpClient.GetAsync($"{UsersRoute}/{id}");
            response.Result.EnsureSuccessStatusCode();
            var model = await response.Result.Content.ReadFromJsonAsync(typeof(User));
            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var user = new User
                {
                    Name = collection["name"],
                    LastName = collection["lastName"],
                    MailAddress = collection["mailAddress"],
                    BirthDate = DateTime.Parse(collection["birthDate"])
                };
               
                HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var httpClient = _httpClientFactory.CreateClient();
                await httpClient.PostAsync(UsersRoute, httpContent);
             
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Edit(Guid id)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = httpClient.GetAsync($"{UsersRoute}/{id}");
            response.Result.EnsureSuccessStatusCode();
            var model = await response.Result.Content.ReadFromJsonAsync(typeof(User));
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, IFormCollection collection)
        {
            try
            {
                var user = new User
                {
                    Id = Guid.Parse(collection["Id"]),
                    Name = collection["name"],
                    LastName = collection["lastName"],
                    MailAddress = collection["mailAddress"],
                    BirthDate = DateTime.Parse(collection["birthDate"])
                };

                HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var httpClient = _httpClientFactory.CreateClient();
                await httpClient.PutAsync($"{UsersRoute}/{id}", httpContent);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Delete(Guid id)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = httpClient.GetAsync($"{UsersRoute}/{id}");
            response.Result.EnsureSuccessStatusCode();
            var model = await response.Result.Content.ReadFromJsonAsync(typeof(User));
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid id, IFormCollection collection)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = httpClient.DeleteAsync($"{UsersRoute}/{id}");
                response.Result.EnsureSuccessStatusCode();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
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
