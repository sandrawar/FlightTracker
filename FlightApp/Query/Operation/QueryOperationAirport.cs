using FlightApp.Query.Processing;
using FlightApp.Query.Processing.Execution;
using FlightApp.Query.Validation;

namespace FlightApp.Query.Operation
{
    internal class QueryOperationAirport : QueryOperationBase
    {
        private static Lazy<IFieldsValidation> fieldsValidationLazy = new Lazy<IFieldsValidation>(
            () => FieldsValidationDirector.Prepare(new AirportFieldsValidationBuilder()));

        public QueryOperationAirport(IFlightAppQueryProcessor flightAppQueryProcessor, IFlightAppQuery nextOperation) : base(flightAppQueryProcessor, QueryObjectClass.Airport, nextOperation)
        {
        }

        protected override IFieldsValidation FieldValidation => fieldsValidationLazy.Value;

        protected override CommandResult ExecuteQuery(FlighAppQueryData query)
            => QueryProcessor.Execute(query.Operation, new QueryAirportExecution(query));
    }
}


