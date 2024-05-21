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

        public FlighAppQueryLibrary(IFlightAppDataQueryRepository flightAppDataQueryRepository, IFlightAppQueryProcessor flightAppQueryProcessor)
        {
            operation = new FlightAppQueryChain()
                .AddAirport(flightAppDataQueryRepository, flightAppQueryProcessor)
                .AddCargo(flightAppDataQueryRepository, flightAppQueryProcessor)
                .AddCargoPlane(flightAppDataQueryRepository, flightAppQueryProcessor)
                .AddCrew(flightAppDataQueryRepository, flightAppQueryProcessor)
                .AddFlight(flightAppDataQueryRepository, flightAppQueryProcessor)
                .AddPassenger(flightAppDataQueryRepository, flightAppQueryProcessor)
                .AddPassengerPlane(flightAppDataQueryRepository, flightAppQueryProcessor)
                .Build();
        }

        public CommandResult Execute(FlighAppQueryData query)
            => operation.Execute(query);
    }
}
