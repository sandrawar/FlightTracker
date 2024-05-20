using FlightApp.Query.Processing;
using FlightApp.Query.Processing.Execution;
using FlightApp.Query.Validation;

namespace FlightApp.Query.Operation
{
    internal class QueryOperationPassenger : QueryOperationBase
    {
        private static Lazy<IFieldsValidation> fieldsValidationLazy = new Lazy<IFieldsValidation>(
            () => FieldsValidationDirector.Prepare(new PassengerFieldsValidationBuilder()));

        public QueryOperationPassenger(IFlightAppQueryProcessor flightAppQueryProcessor, IFlightAppQuery nextOperation) : base(flightAppQueryProcessor, QueryObjectClass.Passenger, nextOperation)
        {
        }

        protected override IFieldsValidation FieldValidation => fieldsValidationLazy.Value;

        protected override CommandResult ExecuteQuery(FlighAppQueryData query)
            => QueryProcessor.Execute(query.Operation, new QueryPassengerExecution(query));
    }
}


