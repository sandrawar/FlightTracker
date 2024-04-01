namespace FlightApp.DataProcessor
{
    internal interface IFlightAppDataProcessor
    {
        IFlightAppCompleteData FlightAppCompleteData { get; }

        void Start();
    }
}
