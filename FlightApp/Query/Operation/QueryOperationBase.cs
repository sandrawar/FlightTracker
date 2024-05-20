using FlightApp.Query.Processing;
using FlightApp.Query.Validation;

namespace FlightApp.Query.Operation
{

    internal abstract class QueryOperationBase : IFlightAppQuery
    {
        private readonly QueryObjectClass objectClass;
        private readonly IFlightAppQuery nextOperationInChain;

        protected QueryOperationBase(
            IFlightAppQueryProcessor flightAppQueryProcessor, 
            QueryObjectClass queryObjectClass, 
            IFlightAppQuery nextOperation)
        {
            QueryProcessor = flightAppQueryProcessor;
            objectClass = queryObjectClass;
            nextOperationInChain = nextOperation;
        }

        protected IFlightAppQueryProcessor QueryProcessor {get; }

        public CommandResult Execute(FlighAppQueryData query)
        {
            if (!ValidateQueryData(query))
            {
                return nextOperationInChain.Execute(query);
            }

            return ExecuteQuery(query);
        }


        protected abstract IFieldsValidation FieldValidation {get; }
        protected abstract CommandResult ExecuteQuery(FlighAppQueryData query);

        private bool ValidateQueryData(FlighAppQueryData query) 
            => query.ObjectClass == objectClass 
            && query.Operation switch
            {
                QueryOperation.Add => query.Values is not null,
                QueryOperation.Delete => true,
                QueryOperation.Display => query.Fields is not null && FieldValidation.ValidateFields(query.Fields),
                QueryOperation.Update => query.Values is not null,
                _ => false
            };
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
