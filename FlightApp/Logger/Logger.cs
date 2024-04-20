namespace FlightApp.Logger
{
    internal interface ILogger
    {
        void LogData(string data);
    }
    internal sealed class Logger : ILogger
    {
        public void LogData(string data)
        {
        }
    }
}
