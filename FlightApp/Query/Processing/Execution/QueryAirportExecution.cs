using FlightApp.DataProcessor;
using System.Globalization;

namespace FlightApp.Query.Processing.Execution
{
    internal class QueryAirportExecution : IQueryExecution<IAirportUpdateDecorator>
    {
        private readonly FlighAppQueryData queryData;

        private static readonly Lazy<IDictionary<string, Func<IAirportUpdateDecorator, string>>> sourceReadLazy = 
            new Lazy<IDictionary<string, Func<IAirportUpdateDecorator, string>>>(() =>
            new Dictionary<string, Func<IAirportUpdateDecorator, string>>()
            {
                 {QuerySyntax.Airport.IdField, airport => airport.Id.ToString(CultureInfo.InvariantCulture) },
                 {QuerySyntax.Airport.AmslField, airport => airport.AMSL.ToString() },
                 {QuerySyntax.Airport.CodeField, airport => airport.Code },
                 {QuerySyntax.Airport.CountryCodeField, airport => airport.Country },
                 {QuerySyntax.Airport.NameField, airport => airport.Name },
                 {QuerySyntax.Airport.WorldPositionField, airport => $"{{{airport.Latitude}, {airport.Longitude}}}" },
            });

        public QueryAirportExecution(FlighAppQueryData flighAppQueryData)
        {
            queryData = flighAppQueryData;
        }

        public IEnumerable<IAirportUpdateDecorator> SelectSource(IFlightAppDataQueryRepository flightAppData) => flightAppData.GetAirportsUpdate();

        public bool IsConditionMet(IAirportUpdateDecorator source) => true;

        public bool Update(IAirportUpdateDecorator source) => true;

        public bool Delete(IAirportUpdateDecorator source) => true;

        public CommandResult PrepareDisplayTable(IEnumerable<IAirportUpdateDecorator> source)
        {
            if (queryData.Fields is null)
            {
                throw new InvalidOperationException("query fields not set");
            }

            var table = QueryTableResultBuilder.BuildTable(
                queryData.Fields,
                QuerySyntax.Airport.Fields,
                source,
                sourceReadLazy.Value);

            return new CommandResult(table);
        }

        public CommandResult Add() => new CommandResult(["Airport added"]);
    }
}
