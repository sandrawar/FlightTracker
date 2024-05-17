using FlightApp.DataProcessor;

namespace FlightApp.Query
{
    internal interface IFlighAppQueryProcessor
    {
        CommandResult ExecuteQuery(FlighAppQuery query);
    }

    internal class FlighAppQuery
    {
        public FlighAppQuery(string operation, string objectClass, string? conditions, string? values, string? fields)
        {
            Operation = operation;
            ObjectClass = objectClass;
            Values = values;
            Conditions = conditions;
            Fields = fields;
        }

        public string Operation { get; }
        public string ObjectClass { get; }
        public string? Fields { get; }
        public string? Values { get; }
        public string? Conditions { get; }
    }

    internal class FlighAppQueryProcessor : IFlighAppQueryProcessor
    {
        private readonly IFlightAppCompleteData flightAppData;

        public FlighAppQueryProcessor(IFlightAppCompleteData data)
        {
            flightAppData = data;
        }

        public CommandResult ExecuteQuery(FlighAppQuery query)
        {
            return QueryInfo(query);
        }

        private CommandResult QueryInfo(FlighAppQuery query)
            => new CommandResult([
                $"operation: {query.Operation}",
                $"class: {query.ObjectClass}",
                $"conditions: {query.Conditions}",
                $"values: {query.Values}",
                $"fields: {query.Fields}",
            ]);
    }
}
