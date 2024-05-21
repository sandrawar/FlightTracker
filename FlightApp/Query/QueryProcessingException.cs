namespace FlightApp.Query
{
    internal class QueryProcessingException : Exception
    {
        public QueryProcessingException()
        {
        }

        public QueryProcessingException(string? message) : base(message)
        {
        }

        public QueryProcessingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
