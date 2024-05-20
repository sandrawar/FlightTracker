using FlightApp.DataProcessor;
using System.Globalization;

namespace FlightApp.Query.Processing.Execution
{
    internal class QueryPassengerPlaneExecution : IQueryExecution<IPassangerPlaneUpdateDecorator>
    {
        private readonly FlighAppQueryData queryData;
        private static readonly Lazy<IDictionary<string, Func<IPassangerPlaneUpdateDecorator, string>>> sourceReadlazy =
            new Lazy<IDictionary<string, Func<IPassangerPlaneUpdateDecorator, string>>>(() =>
            new Dictionary<string, Func<IPassangerPlaneUpdateDecorator, string>>()
            {
                 {QuerySyntax.PassengerPlane.IdField, plane => plane.Id.ToString(CultureInfo.InvariantCulture) },
                 {QuerySyntax.PassengerPlane.BusinessClassSizeField, plane => plane.BuisnessClassSize.ToString() },
                 {QuerySyntax.PassengerPlane.CountryCodeField, plane => plane.Country },
                 {QuerySyntax.PassengerPlane.EconomyClassSizeField, plane => plane.EconomyClassSize.ToString() },
                 {QuerySyntax.PassengerPlane.FirstClassSizeField, plane => plane.FirstClassSize.ToString() },
                 {QuerySyntax.PassengerPlane.ModelField, plane => plane.Model },
                 {QuerySyntax.PassengerPlane.SerialField, plane => plane.Serial },
            });

        public QueryPassengerPlaneExecution(FlighAppQueryData flighAppQueryData)
        {
            queryData = flighAppQueryData;
        }

        public IEnumerable<IPassangerPlaneUpdateDecorator> SelectSource(IFlightAppDataQueryRepository flightAppData) => flightAppData.GetPassangerPlanesUpdate();

        public bool IsConditionMet(IPassangerPlaneUpdateDecorator source) => true;

        public bool Update(IPassangerPlaneUpdateDecorator source) => true;

        public bool Delete(IPassangerPlaneUpdateDecorator source) => true;

        public CommandResult PrepareDisplayTable(IEnumerable<IPassangerPlaneUpdateDecorator> source)
        {
            if (queryData.Fields is null)
            {
                throw new InvalidOperationException("query fields not set");
            }

            var table = QueryTableResultBuilder.BuildTable(
                queryData.Fields,
                QuerySyntax.PassengerPlane.Fields,
                source,
                sourceReadlazy.Value);

            return new CommandResult(table);
        }

        public CommandResult Add() => new CommandResult(["Passanger plane added"]);
    }
}
