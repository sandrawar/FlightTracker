using FlightApp.DataProcessor;
using FlightApp.Query.Condition;
using System.Globalization;

namespace FlightApp.Query.Processing.Execution
{
    internal class QueryCrewExecution : QueryExecutionBase, IQueryExecution<ICrewUpdateDecorator>
    {
        private static readonly Lazy<IDictionary<string, Func<ICrewUpdateDecorator, string>>> sourceReadLazy = new(CreateSourceReadDictionary);
        private static readonly Lazy<IDictionary<string, Func<ICrewUpdateDecorator, ConditionCompareOperation, IConstantNode, bool>>> compareLibLazy = new(CreateCompareDictionary);

        public QueryCrewExecution(IFlightAppDataQueryRepository flightAppDataQueryRepository, FlighAppQueryData flighAppQueryData) : base(flightAppDataQueryRepository, flighAppQueryData)
        {
        }

        public IEnumerable<ICrewUpdateDecorator> SelectSource() => QueryRepository.GetCrewMembersUpdate();

        public bool IsConditionMet(ICrewUpdateDecorator source) =>
            QueryData.Conditions is null
                ? true
                : ConditionEvaluator.Evaluate((operation, name, constantNode) => Compare(source, operation, name, constantNode));

        public bool Update(ICrewUpdateDecorator source)
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


        public bool Delete(ICrewUpdateDecorator source) => QueryRepository.Delete(source);

        public CommandResult PrepareDisplayTable(IEnumerable<ICrewUpdateDecorator> source)
        {
            if (QueryData.Fields is null)
            {
                throw new QueryProcessingException("query fields not set");
            }

            var table = QueryTableResultBuilder.BuildTable(
                QueryData.Fields,
                QuerySyntax.Crew.CoreFields,
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

            var newCrew = new CrewUpdateDecorator(new CreateRecordCrew());
            newCrew.ProcessQueryUpdate(QueryData.Values);

            QueryRepository.Add(newCrew);

            return new CommandResult(["Crew added"]);
        } 

        private static bool Compare(ICrewUpdateDecorator crew, ConditionCompareOperation operation, string identifierName, IConstantNode constantNode) =>
            compareLibLazy.Value.TryGetValue(identifierName, out var compare)
                ? compare(crew, operation, constantNode)
                : throw new QueryProcessingException($"{identifierName} is ivalid for {nameof(QuerySyntax.Crew)}");

        private static Dictionary<string, Func<ICrewUpdateDecorator, ConditionCompareOperation, IConstantNode, bool>> CreateCompareDictionary() =>
            new()
            {
                {QuerySyntax.Crew.IdField, (crew, operation, constantNode) => ConditionComparerHelper.Compare(crew.Id, operation, constantNode) },
                {QuerySyntax.Crew.AgeField, (crew, operation, constantNode) => ConditionComparerHelper.Compare(crew.Age, operation, constantNode) },
                {QuerySyntax.Crew.EmailField, (crew, operation, constantNode) => ConditionComparerHelper.Compare(crew.Email, operation, constantNode) },
                {QuerySyntax.Crew.NameField, (crew, operation, constantNode) => ConditionComparerHelper.Compare(crew.Name, operation, constantNode) },
                {QuerySyntax.Crew.PhoneField, (crew, operation, constantNode) => ConditionComparerHelper.Compare(crew.Phone, operation, constantNode) },
                {QuerySyntax.Crew.PractiseField, (crew, operation, constantNode) => ConditionComparerHelper.Compare(crew.Practice, operation, constantNode) },
                {QuerySyntax.Crew.RoleField, (crew, operation, constantNode) => ConditionComparerHelper.Compare(crew.Role, operation, constantNode) },
            };

        private static Dictionary<string, Func<ICrewUpdateDecorator, string>> CreateSourceReadDictionary() =>
            new()
            {
                {QuerySyntax.Crew.IdField, crew => crew.Id.ToString(CultureInfo.InvariantCulture) },
                {QuerySyntax.Crew.AgeField, crew => crew.Age.ToString(CultureInfo.InvariantCulture) },
                {QuerySyntax.Crew.EmailField, crew => crew.Email },
                {QuerySyntax.Crew.NameField, crew => crew.Name },
                {QuerySyntax.Crew.PhoneField, crew => crew.Phone },
                {QuerySyntax.Crew.PractiseField, crew => crew.Practice.ToString(CultureInfo.InvariantCulture) },
                {QuerySyntax.Crew.RoleField, crew => crew.Role },
            };
    }
}
