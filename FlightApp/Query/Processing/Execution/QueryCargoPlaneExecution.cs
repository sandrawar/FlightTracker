using FlightApp.DataProcessor;
using System.Globalization;

namespace FlightApp.Query.Processing.Execution
{
    internal class QueryCargoPlaneExecution : IQueryExecution<ICargoPlaneUpdateDecorator>
    {
        private readonly FlighAppQueryData queryData;
        private static readonly Lazy<IDictionary<string, Func<ICargoPlaneUpdateDecorator, string>>> sourceReadLazy =
            new Lazy<IDictionary<string, Func<ICargoPlaneUpdateDecorator, string>>>(() =>
            new Dictionary<string, Func<ICargoPlaneUpdateDecorator, string>>()
            {
                 {QuerySyntax.CargoPlane.IdField, plane => plane.Id.ToString(CultureInfo.InvariantCulture) },
                 {QuerySyntax.CargoPlane.CountryCodeField, plane => plane.Country },
                 {QuerySyntax.CargoPlane.MaxLoadField, plane => plane.MaxLoad.ToString() },
                 {QuerySyntax.CargoPlane.ModelField, plane => plane.Model },
                 {QuerySyntax.CargoPlane.SerialField, plane => plane.Serial },
            });

        public QueryCargoPlaneExecution(FlighAppQueryData flighAppQueryData)
        {
            queryData = flighAppQueryData;
        }

        public IEnumerable<ICargoPlaneUpdateDecorator> SelectSource(IFlightAppDataQueryRepository flightAppData) => flightAppData.GetCargoPlanesUpdate();

        public bool IsConditionMet(ICargoPlaneUpdateDecorator source) => true;

        public bool Update(ICargoPlaneUpdateDecorator source) => true;

        public bool Delete(ICargoPlaneUpdateDecorator source) => true;

        public CommandResult PrepareDisplayTable(IEnumerable<ICargoPlaneUpdateDecorator> source)
        {
            if (queryData.Fields is null)
            {
                throw new InvalidOperationException("query fields not set");
            }

            var table = QueryTableResultBuilder.BuildTable(
                queryData.Fields,
                QuerySyntax.CargoPlane.Fields,
                source,
                sourceReadLazy.Value);

            return new CommandResult(table);
        }

        public CommandResult Add() => new CommandResult(["Cargo plane added"]);
    }
}
