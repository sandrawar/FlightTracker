using FlightApp.DataProcessor;
using FlightApp.Query.Condition;
using System.Globalization;

namespace FlightApp.Query.Processing.Execution
{
    internal class QueryPassengerExecution : QueryExecutionBase, IQueryExecution<IPassengerUpdateDecorator>
    {
        private static readonly Lazy<IDictionary<string, Func<IPassengerUpdateDecorator, string>>> sourceReadlazy = new (CreateSourceReadDictionary);
        private static readonly Lazy<IDictionary<string, Func<IPassengerUpdateDecorator, ConditionCompareOperation, IConstantNode, bool>>> compareLibLazy = new(CreateCompareDictionary);

        public QueryPassengerExecution(IFlightAppDataQueryRepository flightAppDataQueryRepository, FlighAppQueryData flighAppQueryData): base(flightAppDataQueryRepository, flighAppQueryData)
        {
        }

        public IEnumerable<IPassengerUpdateDecorator> SelectSource() => QueryRepository.GetPassengersUpdate();

        public bool IsConditionMet(IPassengerUpdateDecorator source) => 
            QueryData.Conditions is null
                ? true
                : ConditionEvaluator.Evaluate((operation, name, constantNode) => Compare(source, operation, name, constantNode));

        public bool Update(IPassengerUpdateDecorator source)
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

            var newPasseneger = new PassengerUpdateDecorator(new CreateRecordPassenger());
            newPasseneger.ProcessQueryUpdate(QueryData.Values);

            QueryRepository.Add(newPasseneger);

            return new CommandResult(["Passenger added"]);
        } 

        private static bool Compare(IPassengerUpdateDecorator passenger, ConditionCompareOperation operation, string identifierName, IConstantNode constantNode) =>
            compareLibLazy.Value.TryGetValue(identifierName, out var compare)
                ? compare(passenger, operation, constantNode)
                : throw new QueryProcessingException($"{identifierName} is ivalid for {nameof(QuerySyntax.Passenger)}");

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
                {QuerySyntax.Passenger.AgeField, passenger => passenger.Age.ToString(CultureInfo.InvariantCulture) },
                {QuerySyntax.Passenger.ClassField, passenger => passenger.Class },
                {QuerySyntax.Passenger.EmailField, passenger => passenger.Email },
                {QuerySyntax.Passenger.MilesField, passenger => passenger.Miles.ToString(CultureInfo.InvariantCulture) },
                {QuerySyntax.Passenger.NameField, passenger => passenger.Name },
                {QuerySyntax.Passenger.PhoneField, passenger => passenger.Phone },
            };
    }
}
