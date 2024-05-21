using FlightApp.DataProcessor;
using FlightApp.Query.Condition;
using System.Globalization;

namespace FlightApp.Query.Processing.Execution
{
    internal class QueryCargoExecution : QueryExecutionBase, IQueryExecution<ICargoUpdateDecorator>
    {
        private static readonly Lazy<IDictionary<string, Func<ICargoUpdateDecorator, string>>> sourceReadLazy = new (CreateSourceReadDictionary);
        private static readonly Lazy<IDictionary<string, Func<ICargoUpdateDecorator, ConditionCompareOperation, IConstantNode, bool>>> compareLibLazy = new (CreateCompareDictionary);
        private static readonly Lazy<IDictionary<string, Action<CreateRecordCargo, string>>> addLibLazy = new (CreateAddDictionary);

        public QueryCargoExecution(IFlightAppDataQueryRepository flightAppDataQueryRepository, FlighAppQueryData flighAppQueryData): base(flightAppDataQueryRepository, flighAppQueryData)
        {
        }

        public IEnumerable<ICargoUpdateDecorator> SelectSource() => QueryRepository.GetCargosUpdate();

        public bool IsConditionMet(ICargoUpdateDecorator source) => 
            QueryData.Conditions is null
                ? true
                : ConditionEvaluator.Evaluate((operation, name, constantNode) => Compare(source, operation, name, constantNode));

        public bool Update(ICargoUpdateDecorator source)
        {
            if (QueryData.Values is null)
            {
                return true;
            }

            if (QueryData.Values.TryGetValue(QuerySyntax.Airport.IdField, out var newIdValue)
                && ulong.TryParse(newIdValue, CultureInfo.CurrentUICulture, out var newId))
            {
                QueryRepository.UpdateData(new IDUpdateData(source.Id, newId));
            }

            return true;
        }


        public bool Delete(ICargoUpdateDecorator source) => QueryRepository.Delete(source);

        public CommandResult PrepareDisplayTable(IEnumerable<ICargoUpdateDecorator> source)
        {
            if (QueryData.Fields is null)
            {
                throw new QueryProcessingException("query fields not set");
            }

            var table = QueryTableResultBuilder.BuildTable(
                QueryData.Fields,
                QuerySyntax.Cargo.CoreFields,
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

            var newCargo = new CreateRecordCargo();
            foreach (var setValue in QueryData.Values)
            {
                if (addLibLazy.Value.TryGetValue(setValue.Key, out var setFunc))
                {
                    try
                    {
                        setFunc(newCargo, setValue.Value);
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

            QueryRepository.Add(newCargo);

            return new CommandResult(["Cargo added"]);
        } 

        private static bool Compare(ICargoUpdateDecorator cargo, ConditionCompareOperation operation, string identifierName, IConstantNode constantNode) =>
            compareLibLazy.Value.TryGetValue(identifierName, out var compare)
                ? compare(cargo, operation, constantNode)
                : throw new QueryProcessingException($"{identifierName} is ivalid for {nameof(QuerySyntax.Cargo)}");

        private static Dictionary<string, Action<CreateRecordCargo, string>> CreateAddDictionary() =>
            new ()
            {
                {QuerySyntax.Cargo.IdField, (cargo, value) => cargo.Id = ulong.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.Cargo.CodeField, (cargo, value) => cargo.Code = value },
                {QuerySyntax.Cargo.DescriptionField, (cargo, value) => cargo.Description = value },
                {QuerySyntax.Cargo.WeightField, (cargo, value) => cargo.Weight = float.Parse(value, CultureInfo.CurrentUICulture) },
            };

        private static Dictionary<string, Func<ICargoUpdateDecorator, ConditionCompareOperation, IConstantNode, bool>> CreateCompareDictionary() =>
            new ()
            {
                {QuerySyntax.Cargo.IdField, (cargo, operation, constantNode) => ConditionComparerHelper.Compare(cargo.Id, operation, constantNode) },
                {QuerySyntax.Cargo.CodeField, (cargo, operation, constantNode) => ConditionComparerHelper.Compare(cargo.Code, operation, constantNode) },
                {QuerySyntax.Cargo.DescriptionField, (cargo, operation, constantNode) => ConditionComparerHelper.Compare(cargo.Description, operation, constantNode) },
                {QuerySyntax.Cargo.WeightField, (cargo, operation, constantNode) => ConditionComparerHelper.Compare(cargo.Weight, operation, constantNode) },
            };

        private static Dictionary<string, Func<ICargoUpdateDecorator, string>> CreateSourceReadDictionary() =>
            new ()
            {
                {QuerySyntax.Cargo.IdField, cargo => cargo.Id.ToString(CultureInfo.InvariantCulture) },
                {QuerySyntax.Cargo.CodeField, cargo => cargo.Code },
                {QuerySyntax.Cargo.DescriptionField, cargo => cargo.Description },
                {QuerySyntax.Cargo.WeightField, cargo => cargo.Weight.ToString() },
            };
    }
}
