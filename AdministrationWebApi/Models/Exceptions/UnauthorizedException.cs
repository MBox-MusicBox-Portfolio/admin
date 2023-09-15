namespace AdministrationWebApi.Models.Exceptions
{
    public class UnauthorizedException : CommonException
    {
        public UnauthorizedException(object response) : base("Unauthorized", response)
        {
        }

        public UnauthorizedException(string message, object response) : base(message, response)
        {
        }
    }

}
