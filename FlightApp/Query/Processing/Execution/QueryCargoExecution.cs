using FlightApp.DataProcessor;
using System.Globalization;

namespace FlightApp.Query.Processing.Execution
{
    internal class QueryCargoExecution : IQueryExecution<ICargoUpdateDecorator>
    {
        private readonly FlighAppQueryData queryData;
        private static readonly Lazy<IDictionary<string, Func<ICargoUpdateDecorator, string>>> sourceReadLazy =
            new Lazy<IDictionary<string, Func<ICargoUpdateDecorator, string>>>(() =>
            new Dictionary<string, Func<ICargoUpdateDecorator, string>>()
            {
                 {QuerySyntax.Cargo.IdField, cargo => cargo.Id.ToString(CultureInfo.InvariantCulture) },
                 {QuerySyntax.Cargo.CodeField, cargo => cargo.Code },
                 {QuerySyntax.Cargo.DescriptionField, cargo => cargo.Description },
                 {QuerySyntax.Cargo.WeightField, cargo => cargo.Weight.ToString() },
            });

        public QueryCargoExecution(FlighAppQueryData flighAppQueryData)
        {
            queryData = flighAppQueryData;
        }

        public IEnumerable<ICargoUpdateDecorator> SelectSource(IFlightAppDataQueryRepository flightAppData) => flightAppData.GetCargosUpdate();
            
        public bool IsConditionMet(ICargoUpdateDecorator source) => true;

        public bool Update(ICargoUpdateDecorator source) => true;

        public bool Delete(ICargoUpdateDecorator source) => true;

        public CommandResult PrepareDisplayTable(IEnumerable<ICargoUpdateDecorator> source)
        {
            if (queryData.Fields is null)
            {
                throw new InvalidOperationException("query fields not set");
            }

            var table = QueryTableResultBuilder.BuildTable(
                queryData.Fields,
                QuerySyntax.Cargo.Fields,
                source,
                sourceReadLazy.Value);

            return new CommandResult(table);
        }

        public CommandResult Add() => new CommandResult(["Cargo added"]);
    }
}
