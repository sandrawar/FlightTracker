namespace FlightApp;

internal interface IFlightAppObjectReader<TRawData>
{
    FlightAppObject Read(TRawData data);
}
