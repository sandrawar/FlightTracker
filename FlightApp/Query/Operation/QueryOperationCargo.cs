using FlightApp.Query.Processing;
using FlightApp.Query.Processing.Execution;

namespace FlightApp.Query.Operation
{
    internal class QueryOperationCargo : QueryOperationBase
    {
        public QueryOperationCargo(IFlightAppQueryProcessor flightAppQueryProcessor, IFlightAppQuery nextOperation) : base(flightAppQueryProcessor, QueryObjectClass.Cargo, nextOperation)
        {
        }

        protected override CommandResult ExecuteQuery(FlighAppQueryData query)
            => QueryProcessor.Execute(query.Operation, new QueryCargoExecution(query));
    }
}


