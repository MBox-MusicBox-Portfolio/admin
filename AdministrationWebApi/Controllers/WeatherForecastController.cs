using AdministrationWebApi.Models.Db;
using Microsoft.AspNetCore.Mvc;
using AdministrationWebApi.Models.RabbitMq;
using AdministrationWebApi.Services.RabbitMQ;
using AdministrationWebApi.Services.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace AdministrationWebApi.Controllers
{
    [ApiController]
    [Route("/api/admin/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
        
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, RabbitMqService rabbit, IConfiguration configuration, IHubContext<NotificationSignalR> hubContext, AppDb db)
        {
            _logger = logger;
            _configuration = configuration;
            _rabbit = rabbit;
            _hubContext = hubContext;
            _db = db;
        }

        private readonly AppDb _db;
        private readonly IConfiguration _configuration;
        private readonly RabbitMqService _rabbit;
        private readonly IHubContext<NotificationSignalR> _hubContext;

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var id = Guid.NewGuid();

            var eventObj = new EventRoute()
            {
                From = ": string | null",
                Body = new
                {
                    Email = "victorgolova@gmail.com",
                    Template = "user_delete",
                    Name = "my name",
                },
                Template = "user_delete"

            };
            _rabbit.SendMessage(eventObj, _configuration["Queue:MAILER"]);
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        
        [HttpGet("{id}/signal")]
        public async Task<IActionResult> GetSignalRAsync(string id)
        {
            await _hubContext.Clients.Group(id).SendAsync("ReceiveMessage", "message");
            return Ok();
        }
    }
}