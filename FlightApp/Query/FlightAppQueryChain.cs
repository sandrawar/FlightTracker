using FlightApp.Chain;
using FlightApp.Query.Operation;
using FlightApp.Query.Processing;

namespace FlightApp.Query
{
    internal class FlightAppQueryChain : Chain<IFlightAppQuery>
    {
        public FlightAppQueryChain() : base(() => new QueryChainTermination(["Query syntax error"]))
        {
        }

        public FlightAppQueryChain AddAirport(IFlightAppQueryProcessor flightAppQueryProcessor)
        {
            AddLink(next => new QueryOperationAirport(flightAppQueryProcessor, next));
            return this;
        }

        public FlightAppQueryChain AddCargo(IFlightAppQueryProcessor flightAppQueryProcessor)
        {
            AddLink(next => new QueryOperationCargo(flightAppQueryProcessor, next));
            return this;
        }

        public FlightAppQueryChain AddCargoPlane(IFlightAppQueryProcessor flightAppQueryProcessor)
        {
            AddLink(next => new QueryOperationCargoPlane(flightAppQueryProcessor, next));
            return this;
        }

        public FlightAppQueryChain AddCrew(IFlightAppQueryProcessor flightAppQueryProcessor)
        {
            AddLink(next => new QueryOperationCrew(flightAppQueryProcessor, next));
            return this;
        }

        public FlightAppQueryChain AddFlight(IFlightAppQueryProcessor flightAppQueryProcessor)
        {
            AddLink(next => new QueryOperationFlight(flightAppQueryProcessor, next));
            return this;
        }

        public FlightAppQueryChain AddPassenger(IFlightAppQueryProcessor flightAppQueryProcessor)
        {
            AddLink(next => new QueryOperationPassenger(flightAppQueryProcessor, next));
            return this;
        }

        public FlightAppQueryChain AddPassengerPlane(IFlightAppQueryProcessor flightAppQueryProcessor)
        {
            AddLink(next => new QueryOperationPassengerPlane(flightAppQueryProcessor, next));
            return this;
        }
    }
}
