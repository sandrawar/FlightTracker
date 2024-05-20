using FlightApp.Query.Processing;
using FlightApp.Query.Processing.Execution;

namespace FlightApp.Query.Operation
{
    internal class QueryOperationAirport : QueryOperationBase
    {
        public QueryOperationAirport(IFlightAppQueryProcessor flightAppQueryProcessor, IFlightAppQuery nextOperation) : base(flightAppQueryProcessor, QueryObjectClass.Airport, nextOperation)
        {
        }

        protected override CommandResult ExecuteQuery(FlighAppQueryData query)
            => QueryProcessor.Execute(query.Operation, new QueryAirportExecution(query));
    }
}


