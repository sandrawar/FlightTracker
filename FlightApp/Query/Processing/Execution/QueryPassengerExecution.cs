using FlightApp.DataProcessor;
using FlightApp.Query.Condition;
using System.Globalization;

namespace FlightApp.Query.Processing.Execution
{
    internal class QueryPassengerExecution : QueryExecutionBase, IQueryExecution<IPassengerUpdateDecorator>
    {
        private static readonly Lazy<IDictionary<string, Func<IPassengerUpdateDecorator, string>>> sourceReadlazy = new (CreateSourceReadDictionary);
        private static readonly Lazy<IDictionary<string, Func<IPassengerUpdateDecorator, ConditionCompareOperation, IConstantNode, bool>>> compareLibLazy = new(CreateCompareDictionary);
        private static readonly Lazy<IDictionary<string, Action<CreateRecordPassenger, string>>> addLibLazy = new (CreateAddDictionary);

        public QueryPassengerExecution(IFlightAppDataQueryRepository flightAppDataQueryRepository, FlighAppQueryData flighAppQueryData): base(flightAppDataQueryRepository, flighAppQueryData)
        {
        }

        public IEnumerable<IPassengerUpdateDecorator> SelectSource() => QueryRepository.GetPassengersUpdate();

        public bool IsConditionMet(IPassengerUpdateDecorator source) => 
            QueryData.Conditions is null
                ? true
                : ConditionEvaluator.Evaluate((operation, name, constantNode) => Compare(source, operation, name, constantNode));

        public bool Update(IPassengerUpdateDecorator source) => true;

        public bool Delete(IPassengerUpdateDecorator source) => QueryRepository.Delete(source);

        public CommandResult PrepareDisplayTable(IEnumerable<IPassengerUpdateDecorator> source)
        {
            if (QueryData.Fields is null)
            {
                throw new QueryProcessingException("query fields not set");
            }

            var table = QueryTableResultBuilder.BuildTable(
                QueryData.Fields,
                QuerySyntax.Passenger.CoreFields,
                source,
                sourceReadlazy.Value);

            return new CommandResult(table);
        }

        public CommandResult Add()
        {
            if (QueryData.Values is null)
            {
                throw new QueryProcessingException("query values not set");
            }

            var newPasseneger = new CreateRecordPassenger();
            foreach (var setValue in QueryData.Values)
            {
                if (addLibLazy.Value.TryGetValue(setValue.Key, out var setFunc))
                {
                    try
                    {
                        setFunc(newPasseneger, setValue.Value);
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

            QueryRepository.Add(newPasseneger);

            return new CommandResult(["Passenger added"]);
        } 

        private static bool Compare(IPassengerUpdateDecorator passenger, ConditionCompareOperation operation, string identifierName, IConstantNode constantNode) =>
            compareLibLazy.Value.TryGetValue(identifierName, out var compare)
                ? compare(passenger, operation, constantNode)
                : throw new QueryProcessingException($"{identifierName} is ivalid for {nameof(QuerySyntax.Passenger)}");

        private static Dictionary<string, Action<CreateRecordPassenger, string>> CreateAddDictionary() =>
            new ()
            {
                {QuerySyntax.Crew.IdField, (passenger, value) => passenger.Id = ulong.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.Crew.AgeField, (passenger, value) => passenger.Age = ulong.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.Passenger.ClassField, (passenger, value) => passenger.Class = value },
                {QuerySyntax.Crew.EmailField, (passenger, value) => passenger.Email = value },
                {QuerySyntax.Passenger.MilesField, (passenger, value) => passenger.Miles = ulong.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.Crew.NameField, (passenger, value) => passenger.Name = value },
                {QuerySyntax.Crew.PhoneField, (passenger, value) => passenger.Phone = value },
            };

        private static Dictionary<string, Func<IPassengerUpdateDecorator, ConditionCompareOperation, IConstantNode, bool>> CreateCompareDictionary() =>
            new()
            {
                {QuerySyntax.Passenger.IdField, (passenger, operation, constantNode) => ConditionComparerHelper.Compare(passenger.Id, operation, constantNode) },
                {QuerySyntax.Passenger.AgeField, (passenger, operation, constantNode) => ConditionComparerHelper.Compare(passenger.Age, operation, constantNode) },
                {QuerySyntax.Passenger.ClassField, (passenger, operation, constantNode) => ConditionComparerHelper.Compare(passenger.Class, operation, constantNode) },
                {QuerySyntax.Passenger.EmailField, (passenger, operation, constantNode) => ConditionComparerHelper.Compare(passenger.Email, operation, constantNode) },
                {QuerySyntax.Passenger.MilesField, (passenger, operation, constantNode) => ConditionComparerHelper.Compare(passenger.Miles, operation, constantNode) },
                {QuerySyntax.Passenger.NameField, (passenger, operation, constantNode) => ConditionComparerHelper.Compare(passenger.Name, operation, constantNode) },
                {QuerySyntax.Passenger.PhoneField, (passenger, operation, constantNode) => ConditionComparerHelper.Compare(passenger.Phone, operation, constantNode) },
            };
        private static Dictionary<string, Func<IPassengerUpdateDecorator, string>> CreateSourceReadDictionary() =>
            new ()
            {
                {QuerySyntax.Passenger.IdField, passenger => passenger.Id.ToString(CultureInfo.InvariantCulture) },
                {QuerySyntax.Passenger.AgeField, passenger => passenger.Age.ToString() },
                {QuerySyntax.Passenger.ClassField, passenger => passenger.Class },
                {QuerySyntax.Passenger.EmailField, passenger => passenger.Email },
                {QuerySyntax.Passenger.MilesField, passenger => passenger.Miles.ToString() },
                {QuerySyntax.Passenger.NameField, passenger => passenger.Name },
                {QuerySyntax.Passenger.PhoneField, passenger => passenger.Phone },
            };
    }
}
