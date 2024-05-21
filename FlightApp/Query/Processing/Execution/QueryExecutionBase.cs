using FlightApp.DataProcessor;
using FlightApp.Query.Condition;

namespace FlightApp.Query.Processing.Execution
{
    internal abstract class QueryExecutionBase
    {
        private readonly Lazy<IConditionEvaluator> conditionTreeLazy;

        protected QueryExecutionBase(IFlightAppDataQueryRepository flightAppDataQueryRepository, FlighAppQueryData flighAppQueryData)
        {
            QueryRepository = flightAppDataQueryRepository;
            QueryData = flighAppQueryData;

            conditionTreeLazy = new (
                () => flighAppQueryData.Conditions is null ? new ConditionEvaluator(true) : new ConditionParser(flighAppQueryData.Conditions).Parse());
        }

        public IFlightAppDataQueryRepository QueryRepository { get; }

        protected FlighAppQueryData QueryData { get; }

        protected IConditionEvaluator ConditionEvaluator => conditionTreeLazy.Value;
    }
}
