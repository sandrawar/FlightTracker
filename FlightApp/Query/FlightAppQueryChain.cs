using FlightApp.Chain;
using FlightApp.DataProcessor;
using FlightApp.Query.Operation;
using FlightApp.Query.Processing;

namespace FlightApp.Query
{
    internal class FlightAppQueryChain : Chain<IFlightAppQuery>
    {
        public FlightAppQueryChain() : base(() => new QueryChainTermination(["Query syntax error"]))
        {
        }

        public FlightAppQueryChain AddAirport(IFlightAppDataQueryRepository flightAppDataQueryRepository, IFlightAppQueryProcessor flightAppQueryProcessor)
        {
            AddLink(next => new QueryOperationAirport(flightAppDataQueryRepository, flightAppQueryProcessor, next));
            return this;
        }

        public FlightAppQueryChain AddCargo(IFlightAppDataQueryRepository flightAppDataQueryRepository, IFlightAppQueryProcessor flightAppQueryProcessor)
        {
            AddLink(next => new QueryOperationCargo(flightAppDataQueryRepository, flightAppQueryProcessor, next));
            return this;
        }

        public FlightAppQueryChain AddCargoPlane(IFlightAppDataQueryRepository flightAppDataQueryRepository, IFlightAppQueryProcessor flightAppQueryProcessor)
        {
            AddLink(next => new QueryOperationCargoPlane(flightAppDataQueryRepository, flightAppQueryProcessor, next));
            return this;
        }

        public FlightAppQueryChain AddCrew(IFlightAppDataQueryRepository flightAppDataQueryRepository, IFlightAppQueryProcessor flightAppQueryProcessor)
        {
            AddLink(next => new QueryOperationCrew(flightAppDataQueryRepository, flightAppQueryProcessor, next));
            return this;
        }

        public FlightAppQueryChain AddFlight(IFlightAppDataQueryRepository flightAppDataQueryRepository, IFlightAppQueryProcessor flightAppQueryProcessor)
        {
            AddLink(next => new QueryOperationFlight(flightAppDataQueryRepository, flightAppQueryProcessor, next));
            return this;
        }

        public FlightAppQueryChain AddPassenger(IFlightAppDataQueryRepository flightAppDataQueryRepository, IFlightAppQueryProcessor flightAppQueryProcessor)
        {
            AddLink(next => new QueryOperationPassenger(flightAppDataQueryRepository, flightAppQueryProcessor, next));
            return this;
        }

        public FlightAppQueryChain AddPassengerPlane(IFlightAppDataQueryRepository flightAppDataQueryRepository, IFlightAppQueryProcessor flightAppQueryProcessor)
        {
            AddLink(next => new QueryOperationPassengerPlane(flightAppDataQueryRepository, flightAppQueryProcessor, next));
            return this;
        }
    }
}
