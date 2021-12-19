namespace UmesiServer.Exceptions
{
    public class HttpResponseException : Exception
    {
        public HttpResponseException(int status, string message)
            : base(message)
        {
            Status = status;
        }

        public int Status { get; set; }
    }
}
