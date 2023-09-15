namespace AdministrationWebApi.Models.Exceptions
{
    public class NotFoundException : CommonException
    {
        public NotFoundException(object response) : base("Not found", response)
        {
        }

        public NotFoundException(string message, object response) : base(message, response)
        {
        }
    }

}
