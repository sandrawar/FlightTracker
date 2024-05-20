using FlightApp.Query.Processing;
using FlightApp.Query.Processing.Execution;

namespace FlightApp.Query.Operation
{
    internal class QueryOperationCargoPlane : QueryOperationBase
    {
        public QueryOperationCargoPlane(IFlightAppQueryProcessor flightAppQueryProcessor, IFlightAppQuery nextOperation) : base(flightAppQueryProcessor, QueryObjectClass.CargoPlane, nextOperation)
        {
        }

        protected override CommandResult ExecuteQuery(FlighAppQueryData query)
            => QueryProcessor.Execute(query.Operation, new QueryCargoPlaneExecution(query));
    }
}


