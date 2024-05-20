using FlightApp.Query.Processing;

namespace FlightApp.Query.Operation
{

    internal abstract class QueryOperationBase : IFlightAppQuery
    {
        private readonly QueryObjectClass objectClass;
        private readonly IFlightAppQuery nextOperationInChain;

        protected QueryOperationBase(IFlightAppQueryProcessor flightAppQueryProcessor, QueryObjectClass queryObjectClass, IFlightAppQuery nextOperation)
        {
            QueryProcessor = flightAppQueryProcessor;
            objectClass = queryObjectClass;
            nextOperationInChain = nextOperation;
        }

        protected IFlightAppQueryProcessor QueryProcessor {get; init; }

        public CommandResult Execute(FlighAppQueryData query)
        {
            if (!ValidateQueryData(query))
            {
                return nextOperationInChain.Execute(query);
            }

            return ExecuteQuery(query);

            //TODO: remove
            //var res = ExecuteQuery(query);

            //return QueryInfoResult(res, query);
        }


        protected abstract CommandResult ExecuteQuery(FlighAppQueryData query);

        private bool ValidateQueryData(FlighAppQueryData query) 
            => query.ObjectClass == objectClass 
            && query.Operation switch
            {
                QueryOperation.Add => query.Values is not null,
                QueryOperation.Delete => true,
                QueryOperation.Display => query.Fields is not null,
                QueryOperation.Update => query.Values is not null,
                _ => false
            };

        //TODO: remove
        //private CommandResult QueryInfoResult(CommandResult inner, FlighAppQueryData query)
        //    => new CommandResult([
        //        .. inner.Messages,
        //        $"handledBy: {GetType().Name}",
        //        $"operation: {query.Operation}",
        //        $"class: {query.ObjectClass}",
        //        $"conditions: {query.Conditions}",
        //        $"values: {query.Values}",
        //        $"fields: {query.Fields}",
        //    ]);
    }

    internal class QueryChainTermination : IFlightAppQuery
    {
        private readonly IEnumerable<string> messages;

        public QueryChainTermination(IEnumerable<string> resultMessages)
        {
            messages = resultMessages;
        }

        public CommandResult Execute(FlighAppQueryData query) => new CommandResult(messages);
    }

}
