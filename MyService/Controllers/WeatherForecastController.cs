using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MyService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ServiceBusClient _client;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ServiceBusClient client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation("Request WeatherForecast received");
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = WeatherForecast.Summaries[rng.Next(WeatherForecast.Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        public async Task<ActionResult> Post(WeatherForecast weatherForecast)
        {
            var sender = _client.CreateSender("SC2021");

            var msg = new ServiceBusMessage
            {
                Body = new BinaryData(JsonConvert.SerializeObject(weatherForecast))
            };

            await sender.SendMessageAsync(msg);
            return Ok(Activity.Current?.TraceId.ToString());
        }
    }
}
