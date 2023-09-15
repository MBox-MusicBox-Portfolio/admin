using AdministrationWebApi.Models.Db;
using Microsoft.AspNetCore.Mvc;
using AdministrationWebApi.Models.RabbitMq;
using AdministrationWebApi.Services.RabbitMQ;
using System.IdentityModel.Tokens.Jwt;

namespace AdministrationWebApi.Controllers
{
    [ApiController]
    [Route("/api/admin/test")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, RabbitMqService rabbit, IConfiguration configuration,  AppDb db)
        {
            _logger = logger;
            _configuration = configuration;
            _rabbit = rabbit;            
            _db = db;
            
        }

        private readonly AppDb _db;
        private readonly IConfiguration _configuration;
        private readonly RabbitMqService _rabbit;

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> GetAsync()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var tokenS = tokenHandler.ReadJwtToken(token);

                    if (tokenS.Claims.Any(claim => claim.Type == "Role" && claim.Value == "super_admin"))
                    {
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
                        _rabbit.SendMessage(eventObj);                        
                    }
                }
                catch (Exception)
                { }
            }
                        
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        
    }
}