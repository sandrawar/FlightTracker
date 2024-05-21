using FlightApp.DataProcessor;
using FlightApp.Query.Processing;
using FlightApp.Query.Processing.Execution;
using FlightApp.Query.Validation;

namespace FlightApp.Query.Operation
{
    internal class QueryOperationPassenger : QueryOperationBase
    {
        private static Lazy<IFieldsValidation> fieldsValidationLazy = new (
            () => FieldsValidationDirector.Prepare(new PassengerFieldsValidationBuilder()));

        public QueryOperationPassenger(IFlightAppDataQueryRepository flightAppDataQueryRepository, IFlightAppQueryProcessor flightAppQueryProcessor, IFlightAppQuery nextOperation) 
            : base(flightAppDataQueryRepository, flightAppQueryProcessor, QueryObjectClass.Passenger, nextOperation)
        {
        }

        protected override IFieldsValidation FieldValidation => fieldsValidationLazy.Value;

        protected override CommandResult ExecuteQuery(FlighAppQueryData query)
            => QueryProcessor.Execute(query.Operation, new QueryPassengerExecution(QueryRepository, query));
    }
}


