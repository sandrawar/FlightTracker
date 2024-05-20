using FlightApp.Query.Processing;
using FlightApp.Query.Processing.Execution;
using FlightApp.Query.Validation;

namespace FlightApp.Query.Operation
{
    internal class QueryOperationPassengerPlane : QueryOperationBase
    {
        private static Lazy<IFieldsValidation> fieldsValidationLazy = new Lazy<IFieldsValidation>(
            () => FieldsValidationDirector.Prepare(new PassengerPlaneFieldsValidationBuilder()));

        public QueryOperationPassengerPlane(IFlightAppQueryProcessor flightAppQueryProcessor, IFlightAppQuery nextOperation) : base(flightAppQueryProcessor, QueryObjectClass.PassengerPlane, nextOperation)
        {
        }

        protected override IFieldsValidation FieldValidation => fieldsValidationLazy.Value;

        protected override CommandResult ExecuteQuery(FlighAppQueryData query)
            => QueryProcessor.Execute(query.Operation, new QueryPassengerPlaneExecution(query));
    }
}


