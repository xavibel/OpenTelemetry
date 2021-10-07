using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Console;
using Newtonsoft.Json;
using WebPage.Models;

namespace WebPage.Controllers
{
    public class UsersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UsersController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: UsersController
        public async Task<ActionResult> Index()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = httpClient.GetAsync("http://localhost:5000/api/Users");
            response.Result.EnsureSuccessStatusCode();
            var model = await response.Result.Content.ReadFromJsonAsync(typeof(List<User>));

            return View(model);
        }

        // GET: UsersController/Details/5
        public async Task<ActionResult> Details(Guid id)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = httpClient.GetAsync($"http://localhost:5000/api/Users/{id}");
            response.Result.EnsureSuccessStatusCode();
            var model = await response.Result.Content.ReadFromJsonAsync(typeof(User));
            return View(model);
        }

        // GET: UsersController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsersController/Create
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
                await httpClient.PostAsync("http://localhost:5000/api/Users", httpContent);
             
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsersController/Edit/5
        public async Task<ActionResult> Edit(Guid id)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = httpClient.GetAsync($"http://localhost:5000/api/Users/{id}");
            response.Result.EnsureSuccessStatusCode();
            var model = await response.Result.Content.ReadFromJsonAsync(typeof(User));
            return View(model);
        }

        // POST: UsersController/Edit/5
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
                await httpClient.PutAsync($"http://localhost:5000/api/Users/{id}", httpContent);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsersController/Delete/5
        public async Task<ActionResult> Delete(Guid id)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = httpClient.GetAsync($"http://localhost:5000/api/Users/{id}");
            response.Result.EnsureSuccessStatusCode();
            var model = await response.Result.Content.ReadFromJsonAsync(typeof(User));
            return View(model);
        }

        // POST: UsersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid id, IFormCollection collection)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = httpClient.DeleteAsync($"http://localhost:5000/api/Users/{id}");
                response.Result.EnsureSuccessStatusCode();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
