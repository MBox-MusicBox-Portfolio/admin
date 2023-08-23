namespace AdministrationWebApi.Models.RabbitMq
{
    public class EventRoute
    {
        public object Body { get; set; }
        public string? From { get; set; }
        public string Template { get; set; }
        public string? To { get; set; }
    }
}
