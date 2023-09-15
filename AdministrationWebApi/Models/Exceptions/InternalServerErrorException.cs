namespace AdministrationWebApi.Models.Exceptions
{
    public class InternalServerErrorException : CommonException
    {
        public InternalServerErrorException(object response) : base("Internal Server Error", response)
        {
        }

        public InternalServerErrorException(string message, object response) : base(message, response)
        {
        }
    }

}
