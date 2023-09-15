namespace AdministrationWebApi.Models.Exceptions
{
    public class BadRequestException : CommonException
    {
        public BadRequestException(object response) : base("Bad request", response)
        {
        }

        public BadRequestException(string message, object response) : base(message, response)
        {
        }
    }

}
