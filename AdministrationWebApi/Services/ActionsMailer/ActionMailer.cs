using AdministrationWebApi.Models.Db;
using AdministrationWebApi.Models.RabbitMq;
using AdministrationWebApi.Services.RabbitMQ;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;

namespace AdministrationWebApi.Services.ActionsMailer
{
    public class ActionMailer : IActionMailer
    {
        private readonly RabbitMqService _rabbit;
        private readonly IConfiguration _configuration;
        private readonly string? queue;
        public ActionMailer(RabbitMqService rabbit, IConfiguration configuration)
        {
            _rabbit = rabbit;
            _configuration = configuration;
            queue = _configuration["Queue:MAILER"];
        }

        public Task NewsDelete(News news)
        {
            //var msg = new SendObject()
            //{
            //    Email = news.Member?.User?.Email,
            //    Template = _configuration["TemplatePages:DELETE_NEWS"],
            //    Body = new { Text = news.Text }
            //};
            //_rabbit.SendMessage(msg, queue);
            return Task.CompletedTask;
        }

        public Task SongAction(Song song, string? template)
        {
            foreach (var band in song.Performer)
            {
                foreach (var member in band.Members)
                {
                    //var msg = new SendObject()
                    //{
                    //    Email = member?.User?.Email,
                    //    Template = template,
                    //    Body = new { Name = song.Name, Band = band.Name }
                    //};
                    //_rabbit.SendMessage(msg, queue);
                }
            }

            return Task.CompletedTask;
        }
        public Task BandAction(Band band, string? template)
        {
            if (template != null)
            {
                foreach (var member in band.Members)
                {
                    //var msg = new SendObject()
                    //{
                    //    Email = member?.User?.Email,
                    //    Template = template,
                    //    Body = new { Band = band.Name }
                    //};
                    //_rabbit.SendMessage(msg, queue);
                }
            }

            return Task.CompletedTask;
        }

        public async Task UserAction(User user, string? template)
        {
          //  var msg = new SendObject()
          //  {
          //      Email = user.Email,
          //      Template = template,
          //      Name= user.Name,
          //      Body = new { Role=user.Role?.Name }
          //  };
          //_rabbit.SendMessage(msg, queue);
        }
    }
}