using FlightApp.DataProcessor;
using System.Globalization;

namespace FlightApp.Query.Processing.Execution
{
    internal class QueryPassengerExecution : IQueryExecution<IPassangerUpdateDecorator>
    {
        private readonly FlighAppQueryData queryData;
        private static readonly Lazy<IDictionary<string, Func<IPassangerUpdateDecorator, string>>> sourceReadlazy =
            new Lazy<IDictionary<string, Func<IPassangerUpdateDecorator, string>>>(() =>
            new Dictionary<string, Func<IPassangerUpdateDecorator, string>>()
            {
                 {QuerySyntax.Passenger.IdField, passenger => passenger.Id.ToString(CultureInfo.InvariantCulture) },
                 {QuerySyntax.Passenger.AgeField, passenger => passenger.Age.ToString() },
                 {QuerySyntax.Passenger.ClassField, passenger => passenger.Class },
                 {QuerySyntax.Passenger.EmailField, passenger => passenger.Email },
                 {QuerySyntax.Passenger.MilesField, passenger => passenger.Miles.ToString() },
                 {QuerySyntax.Passenger.NameField, passenger => passenger.Name },
                 {QuerySyntax.Passenger.PhoneField, passenger => passenger.Phone },
            });

        public QueryPassengerExecution(FlighAppQueryData flighAppQueryData)
        {
            queryData = flighAppQueryData;
        }

        public IEnumerable<IPassangerUpdateDecorator> SelectSource(IFlightAppDataQueryRepository flightAppData) => flightAppData.GetPassangersUpdate();

        public bool IsConditionMet(IPassangerUpdateDecorator source) => true;

        public bool Update(IPassangerUpdateDecorator source) => true;

        public bool Delete(IPassangerUpdateDecorator source) => true;

        public CommandResult PrepareDisplayTable(IEnumerable<IPassangerUpdateDecorator> source)
        {
            if (queryData.Fields is null)
            {
                throw new InvalidOperationException("query fields not set");
            }

            var table = QueryTableResultBuilder.BuildTable(
                queryData.Fields,
                QuerySyntax.Passenger.Fields,
                source,
                sourceReadlazy.Value);

            return new CommandResult(table);
        }

        public CommandResult Add() => new CommandResult(["Passanger added"]);
    }
}
