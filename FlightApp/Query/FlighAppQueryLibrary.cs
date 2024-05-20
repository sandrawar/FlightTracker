using FlightApp.DataProcessor;
using FlightApp.Query.Processing;

namespace FlightApp.Query
{
    internal interface IFlighAppQueryLibrary : IFlightAppQuery
    {
    }

    internal class FlighAppQueryLibrary : IFlighAppQueryLibrary
    {
        private readonly IFlightAppQuery operation;

        public FlighAppQueryLibrary(IFlightAppQueryProcessor flightAppQueryProcessor)
        {
            operation = new FlightAppQueryChain()
                .AddAirport(flightAppQueryProcessor)
                .AddCargo(flightAppQueryProcessor)
                .AddCargoPlane(flightAppQueryProcessor)
                .AddCrew(flightAppQueryProcessor)
                .AddFlight(flightAppQueryProcessor)
                .AddPassenger(flightAppQueryProcessor)
                .AddPassengerPlane(flightAppQueryProcessor)
                .Build();
        }

        public CommandResult Execute(FlighAppQueryData query)
            => operation.Execute(query);
    }
}
