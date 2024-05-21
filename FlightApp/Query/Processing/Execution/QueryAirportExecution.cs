using FlightApp.DataProcessor;
using FlightApp.Query.Condition;
using System.Globalization;

namespace FlightApp.Query.Processing.Execution
{
    internal class QueryAirportExecution : QueryExecutionBase, IQueryExecution<IAirportUpdateDecorator>
    {
        private static readonly Lazy<IDictionary<string, Func<IAirportUpdateDecorator, string>>> sourceReadLazy = new (CreateSourceReadDictionary);
        private static readonly Lazy<IDictionary<string, Func<IAirportUpdateDecorator, ConditionCompareOperation, IConstantNode, bool>>> compareLibLazy = new (CreateCompareDictionary);

        public QueryAirportExecution(IFlightAppDataQueryRepository flightAppDataQueryRepository, FlighAppQueryData flighAppQueryData): base(flightAppDataQueryRepository, flighAppQueryData)
        {
        }

        public IEnumerable<IAirportUpdateDecorator> SelectSource() => QueryRepository.GetAirportsUpdate();

        public bool IsConditionMet(IAirportUpdateDecorator source) => 
            QueryData.Conditions is null
                ? true
                : ConditionEvaluator.Evaluate((operation, name, constantNode) => Compare(source, operation, name, constantNode));

        public bool Update(IAirportUpdateDecorator source)
        {
            if (QueryData.Values is null)
            {
                return true;
            }

            if (QueryData.Values.TryGetValue(QuerySyntax.Airport.IdField, out var newIdValue)
                && ulong.TryParse(newIdValue, CultureInfo.CurrentUICulture, out var newId))
            {
                QueryRepository.UpdateData(new IDUpdateData(source.Id, newId));
            }

            source.ProcessQueryUpdate(QueryData.Values);


            return true;
        }

        public bool Delete(IAirportUpdateDecorator source) => QueryRepository.Delete(source);

        public CommandResult PrepareDisplayTable(IEnumerable<IAirportUpdateDecorator> source)
        {
            if (QueryData.Fields is null)
            {
                throw new QueryProcessingException("query fields not set");
            }

            var table = QueryTableResultBuilder.BuildTable(
                QueryData.Fields,
                QuerySyntax.Airport.CoreFields,
                source,
                sourceReadLazy.Value);

            return new CommandResult(table);
        }

        public CommandResult Add()
        {
            if (QueryData.Values is null)
            {
                throw new QueryProcessingException("query values not set");
            }

            var newAirport = new AirportUpdateDecorator(new CreateRecordAirport());
            newAirport.ProcessQueryUpdate(QueryData.Values);

            QueryRepository.Add(newAirport);

            return new CommandResult(["Airport added"]);
        } 

        private static bool Compare(IAirportUpdateDecorator airport, ConditionCompareOperation operation, string identifierName, IConstantNode constantNode) =>
            compareLibLazy.Value.TryGetValue(identifierName, out var compare)
                ? compare(airport, operation, constantNode)
                : throw new QueryProcessingException($"{identifierName} is ivalid for {nameof(QuerySyntax.Airport)}");

        private static Dictionary<string, Func<IAirportUpdateDecorator, ConditionCompareOperation, IConstantNode, bool>> CreateCompareDictionary() =>
            new ()
            {
                {QuerySyntax.Airport.IdField, (airport, operation, constantNode) => ConditionComparerHelper.Compare(airport.Id, operation, constantNode) },
                {QuerySyntax.Airport.AmslField, (airport, operation, constantNode) => ConditionComparerHelper.Compare(airport.AMSL, operation, constantNode) },
                {QuerySyntax.Airport.CodeField, (airport, operation, constantNode) => ConditionComparerHelper.Compare(airport.Code, operation, constantNode) },
                {QuerySyntax.Airport.CountryCodeField, (airport, operation, constantNode) => ConditionComparerHelper.Compare(airport.Country, operation, constantNode) },
                {QuerySyntax.Airport.NameField, (airport, operation, constantNode) => ConditionComparerHelper.Compare(airport.Name, operation, constantNode) },
                {$"{QuerySyntax.Airport.WorldPositionField}.{QuerySyntax.WorldPosition.LongitudeField}", (airport, operation, constantNode) => ConditionComparerHelper.Compare(airport.Longitude, operation, constantNode) },
                {$"{QuerySyntax.Airport.WorldPositionField}.{QuerySyntax.WorldPosition.LatitudeField}", (airport, operation, constantNode) => ConditionComparerHelper.Compare(airport.Longitude, operation, constantNode) },
            };

        private static Dictionary<string, Func<IAirportUpdateDecorator, string>> CreateSourceReadDictionary() =>
            new ()
            {
                {QuerySyntax.Airport.IdField, airport => airport.Id.ToString(CultureInfo.InvariantCulture) },
                {QuerySyntax.Airport.AmslField, airport => airport.AMSL.ToString() },
                {QuerySyntax.Airport.CodeField, airport => airport.Code },
                {QuerySyntax.Airport.CountryCodeField, airport => airport.Country },
                {QuerySyntax.Airport.NameField, airport => airport.Name },
                {QuerySyntax.Airport.WorldPositionField, airport => $"{{{airport.Latitude}, {airport.Longitude}}}" },
                {$"{QuerySyntax.Airport.WorldPositionField}.{QuerySyntax.WorldPosition.LongitudeField}", airport => airport.Longitude.ToString(CultureInfo.CurrentUICulture) },
                {$"{QuerySyntax.Airport.WorldPositionField}.{QuerySyntax.WorldPosition.LatitudeField}", airport => airport.Longitude.ToString(CultureInfo.CurrentUICulture) },
            };
    }
}
