using FlightApp.DataProcessor;

namespace FlightApp.Readers;

internal interface IFlightAppObjectReader<TRawData>
{
    void AddToFlightAppCompleteData(TRawData data, IFlightAppCompleteData flightAppCompleteData);
}
