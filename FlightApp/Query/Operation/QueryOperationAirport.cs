using FlightApp.DataProcessor;
using FlightApp.Query.Processing;
using FlightApp.Query.Processing.Execution;
using FlightApp.Query.Validation;

namespace FlightApp.Query.Operation
{
    internal class QueryOperationAirport : QueryOperationBase
    {
        private static Lazy<IFieldsValidation> fieldsValidationLazy = new (
            () => FieldsValidationDirector.Prepare(new AirportFieldsValidationBuilder()));

        public QueryOperationAirport(IFlightAppDataQueryRepository flightAppDataQueryRepository, IFlightAppQueryProcessor flightAppQueryProcessor, IFlightAppQuery nextOperation) : 
            base(flightAppDataQueryRepository, flightAppQueryProcessor, QueryObjectClass.Airport, nextOperation)
        {
        }

        protected override IFieldsValidation FieldValidation => fieldsValidationLazy.Value;

        protected override CommandResult ExecuteQuery(FlighAppQueryData query)
            => QueryProcessor.Execute(query.Operation, new QueryAirportExecution(QueryRepository, query));
    }
}


