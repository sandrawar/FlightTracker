using FlightApp.DataProcessor;
using FlightApp.Query.Processing;
using FlightApp.Query.Processing.Execution;
using FlightApp.Query.Validation;

namespace FlightApp.Query.Operation
{
    internal class QueryOperationCargo : QueryOperationBase
    {
        private static Lazy<IFieldsValidation> fieldsValidationLazy = new (
            () => FieldsValidationDirector.Prepare(new CargoFieldsValidationBuilder()));

        public QueryOperationCargo(IFlightAppDataQueryRepository flightAppDataQueryRepository, IFlightAppQueryProcessor flightAppQueryProcessor, IFlightAppQuery nextOperation) 
            : base(flightAppDataQueryRepository, flightAppQueryProcessor, QueryObjectClass.Cargo, nextOperation)
        {
        }

        protected override IFieldsValidation FieldValidation => fieldsValidationLazy.Value;

        protected override CommandResult ExecuteQuery(FlighAppQueryData query)
            => QueryProcessor.Execute(query.Operation, new QueryCargoExecution(QueryRepository, query));
    }
}


