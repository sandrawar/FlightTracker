using FlightApp.DataProcessor;
using System.Globalization;

namespace FlightApp.Query.Processing.Execution
{
    internal class QueryCrewExecution : IQueryExecution<ICrewUpdateDecorator>
    {
        private readonly FlighAppQueryData queryData;
        private static readonly Lazy<IDictionary<string, Func<ICrewUpdateDecorator, string>>> sourceReadLazy =
            new Lazy<IDictionary<string, Func<ICrewUpdateDecorator, string>>>(() =>
            new Dictionary<string, Func<ICrewUpdateDecorator, string>>()
            {
                 {QuerySyntax.Crew.IdField, crew => crew.Id.ToString(CultureInfo.InvariantCulture) },
                 {QuerySyntax.Crew.AgeField, crew => crew.Age.ToString() },
                 {QuerySyntax.Crew.EmailField, crew => crew.Email },
                 {QuerySyntax.Crew.NameField, crew => crew.Name },
                 {QuerySyntax.Crew.PhoneField, crew => crew.Phone },
                 {QuerySyntax.Crew.PractiseField, crew => crew.Practice.ToString() },
                 {QuerySyntax.Crew.RoleField, crew => crew.Role },
            });

        public QueryCrewExecution(FlighAppQueryData flighAppQueryData)
        {
            queryData = flighAppQueryData;
        }

        public IEnumerable<ICrewUpdateDecorator> SelectSource(IFlightAppDataQueryRepository flightAppData) => flightAppData.GetCrewMembersUpdate();

        public bool IsConditionMet(ICrewUpdateDecorator source) => true;

        public bool Update(ICrewUpdateDecorator source) => true;

        public bool Delete(ICrewUpdateDecorator source) => true;

        public CommandResult PrepareDisplayTable(IEnumerable<ICrewUpdateDecorator> source)
        {
            if (queryData.Fields is null)
            {
                throw new InvalidOperationException("query fields not set");
            }

            var table = QueryTableResultBuilder.BuildTable(
                queryData.Fields,
                QuerySyntax.Crew.Fields,
                source,
                sourceReadLazy.Value);

            return new CommandResult(table);
        }

        public CommandResult Add() => new CommandResult(["Crew added"]);
    }
}
