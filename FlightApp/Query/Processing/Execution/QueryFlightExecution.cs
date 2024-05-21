using FlightApp.DataProcessor;
using FlightApp.Query.Condition;
using System.Globalization;

namespace FlightApp.Query.Processing.Execution
{
    internal class QueryFlightExecution : QueryExecutionBase, IQueryExecution<IFlightUpdateDecorator>
    {
        private static readonly Lazy<IDictionary<string, Func<IFlightUpdateDecorator, string>>> sourceReadLazy = new (CreateSourceReadDictionary);
        private static readonly Lazy<IDictionary<string, Func<IFlightUpdateDecorator, ConditionCompareOperation, IConstantNode, bool>>> compareLibLazy = new (CreateCompareDictionary);

        public QueryFlightExecution(IFlightAppDataQueryRepository flightAppDataQueryRepository, FlighAppQueryData flighAppQueryData): base(flightAppDataQueryRepository, flighAppQueryData)
        {
        }

        public IEnumerable<IFlightUpdateDecorator> SelectSource() => QueryRepository.GetFlightsUpdate();

        public bool IsConditionMet(IFlightUpdateDecorator source) => 
            QueryData.Conditions is null
                ? true
                : ConditionEvaluator.Evaluate((operation, name, constantNode) => Compare(source, operation, name, constantNode));

        public bool Update(IFlightUpdateDecorator source)
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


        public bool Delete(IFlightUpdateDecorator source) => QueryRepository.Delete(source);

        public CommandResult PrepareDisplayTable(IEnumerable<IFlightUpdateDecorator> source)
        {
            if (QueryData.Fields is null)
            {
                throw new QueryProcessingException("query fields not set");
            }

            var table = QueryTableResultBuilder.BuildTable(
                QueryData.Fields,
                QuerySyntax.Flight.CoreFields,
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

            var newFlight = new FlightUpdateDecorator(new CreateRecordFlight());
            newFlight.ProcessQueryUpdate(QueryData.Values);

            QueryRepository.Add(newFlight);

            return new CommandResult(["Flight added"]);
        } 


        private static bool Compare(IFlightUpdateDecorator flight, ConditionCompareOperation operation, string identifierName, IConstantNode constantNode) =>
            compareLibLazy.Value.TryGetValue(identifierName, out var compare)
                ? compare(flight, operation, constantNode)
                : throw new QueryProcessingException($"{identifierName} is ivalid for {nameof(QuerySyntax.Flight)}");

        private static Dictionary<string, Func<IFlightUpdateDecorator, ConditionCompareOperation, IConstantNode, bool>> CreateCompareDictionary() =>
            new()
            {
                {QuerySyntax.Flight.IdField, (flight, operation, constantNode) => ConditionComparerHelper.Compare(flight.Id, operation, constantNode) },
                {QuerySyntax.Flight.AmslField, (flight, operation, constantNode) => ConditionComparerHelper.Compare(flight.AMSL, operation, constantNode) },
                {QuerySyntax.Flight.LandingTimeField, (flight, operation, constantNode) => ConditionComparerHelper.Compare(flight.LandingTime, operation, constantNode) },
                {QuerySyntax.Flight.OriginField, (flight, operation, constantNode) => ConditionComparerHelper.Compare(flight.OriginAsID, operation, constantNode) },
                {QuerySyntax.Flight.PlaneField, (flight, operation, constantNode) => ConditionComparerHelper.Compare(flight.PlaneID, operation, constantNode) },
                {QuerySyntax.Flight.TakeofTimeField, (flight, operation, constantNode) => ConditionComparerHelper.Compare(flight.TakeoffTime, operation, constantNode) },
                {QuerySyntax.Flight.TargetField, (flight, operation, constantNode) => ConditionComparerHelper.Compare(flight.TargetAsID, operation, constantNode) },
                {$"{QuerySyntax.Flight.WorldPositionField}.{QuerySyntax.WorldPosition.LongitudeField}", (flight, operation, constantNode) => ConditionComparerHelper.Compare(flight.Longitude, operation, constantNode) },
                {$"{QuerySyntax.Flight.WorldPositionField}.{QuerySyntax.WorldPosition.LatitudeField}", (flight, operation, constantNode) => ConditionComparerHelper.Compare(flight.Latitude, operation, constantNode) },
            };

        private static Dictionary<string, Func<IFlightUpdateDecorator, string>> CreateSourceReadDictionary() =>
            new ()
            {
                {QuerySyntax.Flight.IdField, flight => flight.Id.ToString(CultureInfo.InvariantCulture) },
                {QuerySyntax.Flight.AmslField, flight => flight.AMSL?.ToString() ?? string.Empty },
                {QuerySyntax.Flight.LandingTimeField, flight => flight.LandingTime.ToString() },
                {QuerySyntax.Flight.OriginField, flight => flight.OriginAsID.ToString(CultureInfo.InvariantCulture) },
                {QuerySyntax.Flight.PlaneField, flight => flight.PlaneID.ToString(CultureInfo.InvariantCulture) },
                {QuerySyntax.Flight.TakeofTimeField, flight => flight.TakeoffTime.ToString() },
                {QuerySyntax.Flight.TargetField, flight => flight.TargetAsID.ToString(CultureInfo.InvariantCulture) },
                {QuerySyntax.Flight.WorldPositionField, flight => $"{{{flight.Latitude}, {flight.Longitude}}}" },
                {$"{QuerySyntax.Flight.WorldPositionField}.{QuerySyntax.WorldPosition.LongitudeField}", flight => $"{flight.Longitude}" },
                {$"{QuerySyntax.Flight.WorldPositionField}.{QuerySyntax.WorldPosition.LatitudeField}", flight => $"{flight.Latitude}" },
            };
    }
}
