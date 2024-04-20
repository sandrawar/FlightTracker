using FlightApp.DataProcessor;

namespace FlightApp.Readers;

internal interface IFlightAppObjectReader<TRawData>
{
    void AddToFlightAppDataUpdate(TRawData data, IFlightAppDataUpdate flightAppCompleteData);


}
