using FlightApp.Query.Processing;
using FlightApp.Query.Processing.Execution;

namespace FlightApp.Query.Operation
{
    internal class QueryOperationCrew : QueryOperationBase
    {
        public QueryOperationCrew(IFlightAppQueryProcessor flightAppQueryProcessor, IFlightAppQuery nextOperation) : base(flightAppQueryProcessor, QueryObjectClass.Crew, nextOperation)
        {
        }

        protected override CommandResult ExecuteQuery(FlighAppQueryData query)
            => QueryProcessor.Execute(query.Operation, new QueryCrewExecution(query));
    }
}


