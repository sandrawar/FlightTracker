using FlightApp.DataProcessor;
using FlightApp.Query.Processing;
using FlightApp.Query.Validation;

namespace FlightApp.Query.Operation
{

    internal abstract class QueryOperationBase : IFlightAppQuery
    {
        private readonly QueryObjectClass objectClass;
        private readonly IFlightAppQuery nextOperationInChain;

        protected QueryOperationBase(
            IFlightAppDataQueryRepository flightAppDataQueryRepository, 
            IFlightAppQueryProcessor flightAppQueryProcessor, 
            QueryObjectClass queryObjectClass, 
            IFlightAppQuery nextOperation)
        {
            QueryRepository = flightAppDataQueryRepository;
            QueryProcessor = flightAppQueryProcessor;
            objectClass = queryObjectClass;
            nextOperationInChain = nextOperation;
        }

        protected IFlightAppDataQueryRepository QueryRepository { get; }
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
                QueryOperation.Add => ValidateQueryValues(query),
                QueryOperation.Delete => ValidateQueryConditions(query),
                QueryOperation.Display => ValidateQueryFields(query) && ValidateQueryConditions(query),
                QueryOperation.Update => ValidateQueryValues(query) && ValidateQueryConditions(query),
                _ => false
            };

        private bool ValidateQueryFields(FlighAppQueryData query) => query.Fields is not null && FieldValidation.ValidateFields(query.Fields);

        private bool ValidateQueryConditions(FlighAppQueryData query) => 
            query.Conditions is null 
            || FieldValidation.ValidateFields(
                query.Conditions.Where(c => c.TokenType == Condition.ConditionTokenType.Identifier)
                .Select(c => c.Value)
                .ToArray());

        private bool ValidateQueryValues(FlighAppQueryData query) =>
            query.Values is not null
            && FieldValidation.ValidateFields(
                query.Values.Keys);
    }

    internal class QueryChainTermination : IFlightAppQuery
    {
        private readonly IReadOnlyCollection<string> messages;

        public QueryChainTermination(IReadOnlyCollection<string> resultMessages)
        {
            messages = resultMessages;
        }

        public CommandResult Execute(FlighAppQueryData query) => new CommandResult(messages);
    }

}
