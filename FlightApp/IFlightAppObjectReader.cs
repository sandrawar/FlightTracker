namespace FlightApp;

internal interface IFlightAppObjectReader<TRawData>
{
    void AddToFlightAppCompleteData(TRawData data, IFlightAppCompleteData flightAppCompleteData);
}
