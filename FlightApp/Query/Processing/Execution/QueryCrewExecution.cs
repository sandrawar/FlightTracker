using FlightApp.DataProcessor;
using FlightApp.Query.Condition;
using System.Globalization;

namespace FlightApp.Query.Processing.Execution
{
    internal class QueryCrewExecution : QueryExecutionBase, IQueryExecution<ICrewUpdateDecorator>
    {
        private static readonly Lazy<IDictionary<string, Func<ICrewUpdateDecorator, string>>> sourceReadLazy = new(CreateSourceReadDictionary);
        private static readonly Lazy<IDictionary<string, Func<ICrewUpdateDecorator, ConditionCompareOperation, IConstantNode, bool>>> compareLibLazy = new(CreateCompareDictionary);
        private static readonly Lazy<IDictionary<string, Action<CreateRecordCrew, string>>> addLibLazy = new (CreateAddDictionary);

        public QueryCrewExecution(IFlightAppDataQueryRepository flightAppDataQueryRepository, FlighAppQueryData flighAppQueryData) : base(flightAppDataQueryRepository, flighAppQueryData)
        {
        }

        public IEnumerable<ICrewUpdateDecorator> SelectSource() => QueryRepository.GetCrewMembersUpdate();

        public bool IsConditionMet(ICrewUpdateDecorator source) =>
            QueryData.Conditions is null
                ? true
                : ConditionEvaluator.Evaluate((operation, name, constantNode) => Compare(source, operation, name, constantNode));

        public bool Update(ICrewUpdateDecorator source) => true;

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

            var newCrew = new CreateRecordCrew();
            foreach (var setValue in QueryData.Values)
            {
                if (addLibLazy.Value.TryGetValue(setValue.Key, out var setFunc))
                {
                    try
                    {
                        setFunc(newCrew, setValue.Value);
                    }
                    catch (Exception ex)
                    { 
                        throw new QueryProcessingException($"setting {setValue.Key} value error", ex);
                    }
                }
                else
                {
                    throw new QueryProcessingException($"setter for {setValue.Key} not found");
                }
            }

            QueryRepository.Add(newCrew);

            return new CommandResult(["Crew added"]);
        } 

        private static bool Compare(ICrewUpdateDecorator crew, ConditionCompareOperation operation, string identifierName, IConstantNode constantNode) =>
            compareLibLazy.Value.TryGetValue(identifierName, out var compare)
                ? compare(crew, operation, constantNode)
                : throw new QueryProcessingException($"{identifierName} is ivalid for {nameof(QuerySyntax.Crew)}");

        private static Dictionary<string, Action<CreateRecordCrew, string>> CreateAddDictionary() =>
            new ()
            {
                {QuerySyntax.Crew.IdField, (crew, value) => crew.Id = ulong.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.Crew.AgeField, (crew, value) => crew.Age = ulong.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.Crew.EmailField, (crew, value) => crew.Email = value },
                {QuerySyntax.Crew.NameField, (crew, value) => crew.Name = value },
                {QuerySyntax.Crew.PhoneField, (crew, value) => crew.Phone = value },
                {QuerySyntax.Crew.PractiseField, (crew, value) => crew.Practice = ushort.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.Crew.RoleField, (crew, value) => crew.Role = value },
            };

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
                {QuerySyntax.Crew.AgeField, crew => crew.Age.ToString() },
                {QuerySyntax.Crew.EmailField, crew => crew.Email },
                {QuerySyntax.Crew.NameField, crew => crew.Name },
                {QuerySyntax.Crew.PhoneField, crew => crew.Phone },
                {QuerySyntax.Crew.PractiseField, crew => crew.Practice.ToString() },
                {QuerySyntax.Crew.RoleField, crew => crew.Role },
            };
    }
}
