namespace AdministrationWebApi.Models.Exceptions
{
    public class CommonException:Exception
    {
        public object Response { get; set; }

        public CommonException(object response)
        {
            Response = response;
        }

        public CommonException(string message, object response) : base(message)
        {
            Response = response;
        }
    }
}
