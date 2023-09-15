namespace AdministrationWebApi.Models.Exceptions
{
    public class UnprocessableEntityException : CommonException
    {
        public UnprocessableEntityException(object response) : base("Unprocessable Entity", response)
        {
        }

        public UnprocessableEntityException(string message, object response) : base(message, response)
        {
        }
    }

}
