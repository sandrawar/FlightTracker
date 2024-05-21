using FlightApp.DataProcessor;
using FlightApp.Query.Processing;
using FlightApp.Query.Processing.Execution;
using FlightApp.Query.Validation;

namespace FlightApp.Query.Operation
{
    internal class QueryOperationCargoPlane : QueryOperationBase
    {
        private static Lazy<IFieldsValidation> fieldsValidationLazy = new (
            () => FieldsValidationDirector.Prepare(new CargoPlaneFieldsValidationBuilder()));

        public QueryOperationCargoPlane(IFlightAppDataQueryRepository flightAppDataQueryRepository, IFlightAppQueryProcessor flightAppQueryProcessor, IFlightAppQuery nextOperation) 
            : base(flightAppDataQueryRepository, flightAppQueryProcessor, QueryObjectClass.CargoPlane, nextOperation)
        {
        }

        protected override IFieldsValidation FieldValidation => fieldsValidationLazy.Value;

        protected override CommandResult ExecuteQuery(FlighAppQueryData query)
            => QueryProcessor.Execute(query.Operation, new QueryCargoPlaneExecution(QueryRepository, query));
    }
}


