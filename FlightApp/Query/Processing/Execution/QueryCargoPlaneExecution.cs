﻿using FlightApp.DataProcessor;
using FlightApp.Query.Condition;
using System.Globalization;

namespace FlightApp.Query.Processing.Execution
{
    internal class QueryCargoPlaneExecution : QueryExecutionBase, IQueryExecution<ICargoPlaneUpdateDecorator>
    {
        private static readonly Lazy<IDictionary<string, Func<ICargoPlaneUpdateDecorator, string>>> sourceReadLazy = new(CreateSourceReadDictionary);
        private static readonly Lazy<IDictionary<string, Func<ICargoPlaneUpdateDecorator, ConditionCompareOperation, IConstantNode, bool>>> compareLibLazy = new(CreateCompareDictionary);
        
        public QueryCargoPlaneExecution(IFlightAppDataQueryRepository flightAppDataQueryRepository, FlighAppQueryData flighAppQueryData) : base(flightAppDataQueryRepository, flighAppQueryData)
        {
        }

        public IEnumerable<ICargoPlaneUpdateDecorator> SelectSource() => QueryRepository.GetCargoPlanesUpdate();

        public bool IsConditionMet(ICargoPlaneUpdateDecorator source) =>
            QueryData.Conditions is null
                ? true
                : ConditionEvaluator.Evaluate((operation, name, constantNode) => Compare(source, operation, name, constantNode));

        public bool Update(ICargoPlaneUpdateDecorator source)
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


        public bool Delete(ICargoPlaneUpdateDecorator source) => QueryRepository.Delete(source);

        public CommandResult PrepareDisplayTable(IEnumerable<ICargoPlaneUpdateDecorator> source)
        {
            if (QueryData.Fields is null)
            {
                throw new QueryProcessingException("query fields not set");
            }

            var table = QueryTableResultBuilder.BuildTable(
                QueryData.Fields,
                QuerySyntax.CargoPlane.CoreFields,
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

            var newCargoPlane = new CargoPlaneUpdateDecorator(new CreateRecordCargoPlane());
            newCargoPlane.ProcessQueryUpdate(QueryData.Values);

            QueryRepository.Add(newCargoPlane);

            return new CommandResult(["Cargo plane added"]);
        } 

        private static bool Compare(ICargoPlaneUpdateDecorator plane, ConditionCompareOperation operation, string identifierName, IConstantNode constantNode) =>
            compareLibLazy.Value.TryGetValue(identifierName, out var compare)
                ? compare(plane, operation, constantNode)
                : throw new QueryProcessingException($"{identifierName} is ivalid for {nameof(QuerySyntax.CargoPlane)}");

        private static Dictionary<string, Func<ICargoPlaneUpdateDecorator, ConditionCompareOperation, IConstantNode, bool>> CreateCompareDictionary() =>
            new()
            {
                {QuerySyntax.CargoPlane.IdField, (plane, operation, constantNode) => ConditionComparerHelper.Compare(plane.Id, operation, constantNode) },
                {QuerySyntax.CargoPlane.CountryCodeField, (plane, operation, constantNode) => ConditionComparerHelper.Compare(plane.Country, operation, constantNode) },
                {QuerySyntax.CargoPlane.MaxLoadField, (plane, operation, constantNode) => ConditionComparerHelper.Compare(plane.MaxLoad, operation, constantNode) },
                {QuerySyntax.CargoPlane.ModelField, (plane, operation, constantNode) => ConditionComparerHelper.Compare(plane.Model, operation, constantNode) },
                {QuerySyntax.CargoPlane.SerialField, (plane, operation, constantNode) => ConditionComparerHelper.Compare(plane.Serial, operation, constantNode) },
            };

        private static Dictionary<string, Func<ICargoPlaneUpdateDecorator, string>> CreateSourceReadDictionary() =>
            new()
            {
                {QuerySyntax.CargoPlane.IdField, plane => plane.Id.ToString(CultureInfo.InvariantCulture) },
                {QuerySyntax.CargoPlane.CountryCodeField, plane => plane.Country },
                {QuerySyntax.CargoPlane.MaxLoadField, plane => plane.MaxLoad.ToString(CultureInfo.InvariantCulture) },
                {QuerySyntax.CargoPlane.ModelField, plane => plane.Model },
                {QuerySyntax.CargoPlane.SerialField, plane => plane.Serial },
            };
    }
}
