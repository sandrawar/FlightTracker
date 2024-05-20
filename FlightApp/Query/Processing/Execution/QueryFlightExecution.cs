using FlightApp.DataProcessor;
using System.Globalization;

namespace FlightApp.Query.Processing.Execution
{
    internal class QueryFlightExecution : IQueryExecution<IFlightUpdateDecorator>
    {
        private readonly FlighAppQueryData queryData;
        private static readonly Lazy<IDictionary<string, Func<IFlightUpdateDecorator, string>>> sourceReadLazy =
            new Lazy<IDictionary<string, Func<IFlightUpdateDecorator, string>>>(() =>
            new Dictionary<string, Func<IFlightUpdateDecorator, string>>()
            {
                 {QuerySyntax.Flight.IdField, flight => flight.Id.ToString(CultureInfo.InvariantCulture) },
                 {QuerySyntax.Flight.AmslField, flight => flight.AMSL?.ToString() ?? string.Empty },
                 {QuerySyntax.Flight.LandingTimeField, flight => flight.LandingTime.ToString() },
                 {QuerySyntax.Flight.OriginField, flight => flight.OriginAsID.ToString(CultureInfo.InvariantCulture) },
                 {QuerySyntax.Flight.PlaneField, flight => flight.PlaneID.ToString(CultureInfo.InvariantCulture) },
                 {QuerySyntax.Flight.TakeofTimeField, flight => flight.TakeoffTime.ToString() },
                 {QuerySyntax.Flight.TargetField, flight => flight.TargetAsID.ToString(CultureInfo.InvariantCulture) },
                 {QuerySyntax.Flight.WorldPositionField, flight => $"{{{flight.Latitude}, {flight.Longitude}}}" },
            });

        public QueryFlightExecution(FlighAppQueryData flighAppQueryData)
        {
            queryData = flighAppQueryData;
        }

        public IEnumerable<IFlightUpdateDecorator> SelectSource(IFlightAppDataQueryRepository flightAppData) => flightAppData.GetFlightsUpdate();

        public bool IsConditionMet(IFlightUpdateDecorator source) => true;

        public bool Update(IFlightUpdateDecorator source) => true;

        public bool Delete(IFlightUpdateDecorator source) => true;

        public CommandResult PrepareDisplayTable(IEnumerable<IFlightUpdateDecorator> source)
        {
            if (queryData.Fields is null)
            {
                throw new InvalidOperationException("query fields not set");
            }

            var table = QueryTableResultBuilder.BuildTable(
                queryData.Fields,
                QuerySyntax.Flight.Fields,
                source,
                sourceReadLazy.Value);

            return new CommandResult(table);
        }

        public CommandResult Add() => new CommandResult(["Flight added"]);
    }
}
