using FlightApp.Query.Processing;
using FlightApp.Query.Processing.Execution;

namespace FlightApp.Query.Operation
{
    internal class QueryOperationPassengerPlane : QueryOperationBase
    {
        public QueryOperationPassengerPlane(IFlightAppQueryProcessor flightAppQueryProcessor, IFlightAppQuery nextOperation) : base(flightAppQueryProcessor, QueryObjectClass.PassengerPlane, nextOperation)
        {
        }

        protected override CommandResult ExecuteQuery(FlighAppQueryData query)
            => QueryProcessor.Execute(query.Operation, new QueryPassengerPlaneExecution(query));
    }
}


