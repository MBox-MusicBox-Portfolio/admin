using AdministrationWebApi.Models.Db;
using AdministrationWebApi.Models.RabbitMq;
using AdministrationWebApi.Services.RabbitMQ;
using System.Diagnostics.Metrics;

namespace AdministrationWebApi.Services.ActionsMailer
{
    public class ActionEventRoute : IActionEventRoute
    {
        private readonly RabbitMqService _rabbit;
        private readonly IConfiguration _configuration;
        public ActionEventRoute(RabbitMqService rabbit, IConfiguration configuration)
        {
            _rabbit = rabbit;
            _configuration = configuration;
        }

        public Task NewsDelete(News news)
        {
            var msg = new EventRoute()
            {
                To = news.Member?.User?.Id.ToString(),
                Template = _configuration["TemplatePages:DELETE_NEWS"]??"",
                Body = new { Text = news.Text, Email = news.Member?.User?.Email, }
            };
            _rabbit.SendMessage(msg);
            return Task.CompletedTask;
        }

        public Task SongAction(Song song, string template)
        {
            foreach (var band in song.Performer)
            {
                var msg = new EventRoute()
                {
                    To = band?.Producer?.User?.Id.ToString(),
                    Template = template,
                    Body = new { Email = band?.Producer?.User?.Email, Band = band?.Name, Name = song.Name }
                };
                _rabbit.SendMessage(msg);               
            }

            return Task.CompletedTask;
        }
        public Task BandAction(Band band, string? template)
        {
            if (template != null)
            {
                var msg = new EventRoute()
                {
                    To = band?.Producer?.User?.Id.ToString(),
                    Template = template,
                    Body = new { Email = band?.Producer?.User?.Email, Band = band.Name }
                };
                _rabbit.SendMessage(msg);
                foreach (var member in band.Members)
                {
                    var msgMembers = new EventRoute()
                    {
                        To = band?.Producer?.User?.Id.ToString(),
                        Template = template,
                        Body = new { Email = member?.User?.Email, Band = band?.Name }
                    };                
                    _rabbit.SendMessage(msgMembers);
                }
            }

            return Task.CompletedTask;
        }

        public Task UserAction(User user, string template)
        {
            if (user != null)
            {
                var msgMembers = new EventRoute()
                {
                    To =user?.Id.ToString(),
                    Template = template,
                    Body = new { user?.Email, Role=user?.Role?.Name, user?.Name }
                };
                _rabbit.SendMessage(msgMembers);
            }

            return Task.CompletedTask;
        }
    }
}