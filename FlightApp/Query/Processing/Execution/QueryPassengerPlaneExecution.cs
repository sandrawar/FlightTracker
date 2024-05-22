using FlightApp.DataProcessor;
using FlightApp.Query.Condition;
using System.Globalization;

namespace FlightApp.Query.Processing.Execution
{
    internal class QueryPassengerPlaneExecution : QueryExecutionBase, IQueryExecution<IPassengerPlaneUpdateDecorator>
    {
        private static readonly Lazy<IDictionary<string, Func<IPassengerPlaneUpdateDecorator, string>>> sourceReadlazy = new (CreateSourceReadDictionary);
        private static readonly Lazy<IDictionary<string, Func<IPassengerPlaneUpdateDecorator, ConditionCompareOperation, IConstantNode, bool>>> compareLibLazy = new(CreateCompareDictionary);

        public QueryPassengerPlaneExecution(IFlightAppDataQueryRepository flightAppDataQueryRepository, FlighAppQueryData flighAppQueryData): base(flightAppDataQueryRepository, flighAppQueryData)
        {
        }

        public IEnumerable<IPassengerPlaneUpdateDecorator> SelectSource() => QueryRepository.GetPassengerPlanesUpdate();

        public bool IsConditionMet(IPassengerPlaneUpdateDecorator source) => 
            QueryData.Conditions is null
                ? true
                : ConditionEvaluator.Evaluate((operation, name, constantNode) => Compare(source, operation, name, constantNode));

        public bool Update(IPassengerPlaneUpdateDecorator source)
        {
            if (QueryData.Values is null)
            {
                return true;
            }

            if (QueryData.Values.TryGetValue(QuerySyntax.Airport.IdField, out var newIdValue)
                && ulong.TryParse(newIdValue, CultureInfo.InvariantCulture, out var newId))
            {
                QueryRepository.UpdateData(new IDUpdateData(source.Id, newId));
            }

            source.ProcessQueryUpdate(QueryData.Values);

            return true;
        }


        public bool Delete(IPassengerPlaneUpdateDecorator source) => QueryRepository.Delete(source);

        public CommandResult PrepareDisplayTable(IEnumerable<IPassengerPlaneUpdateDecorator> source)
        {
            if (QueryData.Fields is null)
            {
                throw new QueryProcessingException("query fields not set");
            }

            var table = QueryTableResultBuilder.BuildTable(
                QueryData.Fields,
                QuerySyntax.PassengerPlane.CoreFields,
                source,
                sourceReadlazy.Value);

            return new CommandResult(table);
        }

        public CommandResult Add()
        {
            if (QueryData.Values is null)
            {
                throw new QueryProcessingException("query values not set");
            }

            var newPassengerPlane = new PassengerPlaneUpdateDecorator(new CreateRecordPassengerPlane());
            newPassengerPlane.ProcessQueryUpdate(QueryData.Values);

            QueryRepository.Add(newPassengerPlane);

            return new CommandResult(["Cargo plane added"]);
        } 

        private static bool Compare(IPassengerPlaneUpdateDecorator plane, ConditionCompareOperation operation, string identifierName, IConstantNode constantNode) =>
            compareLibLazy.Value.TryGetValue(identifierName, out var compare)
                ? compare(plane, operation, constantNode)
                : throw new QueryProcessingException($"{identifierName} is ivalid for {nameof(QuerySyntax.PassengerPlane)}");

        private static Dictionary<string, Func<IPassengerPlaneUpdateDecorator, ConditionCompareOperation, IConstantNode, bool>> CreateCompareDictionary() =>
            new()
            {
                {QuerySyntax.PassengerPlane.IdField, (plane, operation, constantNode) => ConditionComparerHelper.Compare(plane.Id, operation, constantNode) },
                {QuerySyntax.PassengerPlane.BusinessClassSizeField, (plane, operation, constantNode) => ConditionComparerHelper.Compare(plane.BusinessClassSize, operation, constantNode) },
                {QuerySyntax.PassengerPlane.CountryCodeField, (plane, operation, constantNode) => ConditionComparerHelper.Compare(plane.Country, operation, constantNode) },
                {QuerySyntax.PassengerPlane.EconomyClassSizeField, (plane, operation, constantNode) => ConditionComparerHelper.Compare(plane.EconomyClassSize, operation, constantNode) },
                {QuerySyntax.PassengerPlane.FirstClassSizeField, (plane, operation, constantNode) => ConditionComparerHelper.Compare(plane.FirstClassSize, operation, constantNode) },
                {QuerySyntax.PassengerPlane.ModelField, (plane, operation, constantNode) => ConditionComparerHelper.Compare(plane.Model, operation, constantNode) },
                {QuerySyntax.PassengerPlane.SerialField, (plane, operation, constantNode) => ConditionComparerHelper.Compare(plane.Serial, operation, constantNode) },
            };

        private static Dictionary<string, Func<IPassengerPlaneUpdateDecorator, string>> CreateSourceReadDictionary() =>
            new ()
            {
                {QuerySyntax.PassengerPlane.IdField, plane => plane.Id.ToString(CultureInfo.InvariantCulture) },
                {QuerySyntax.PassengerPlane.BusinessClassSizeField, plane => plane.BusinessClassSize.ToString(CultureInfo.InvariantCulture) },
                {QuerySyntax.PassengerPlane.CountryCodeField, plane => plane.Country },
                {QuerySyntax.PassengerPlane.EconomyClassSizeField, plane => plane.EconomyClassSize.ToString(CultureInfo.InvariantCulture) },
                {QuerySyntax.PassengerPlane.FirstClassSizeField, plane => plane.FirstClassSize.ToString(CultureInfo.InvariantCulture) },
                {QuerySyntax.PassengerPlane.ModelField, plane => plane.Model },
                {QuerySyntax.PassengerPlane.SerialField, plane => plane.Serial },
            };
    }
}
