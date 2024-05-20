using FlightApp.DataProcessor;

namespace FlightApp.Query.Processing.Execution
{
    internal class QueryFlightExecution : IQueryExecution<IFlightUpdateDecorator>
    {
        private readonly FlighAppQueryData queryData;

        public QueryFlightExecution(FlighAppQueryData flighAppQueryData)
        {
            queryData = flighAppQueryData;
        }

        public IEnumerable<IFlightUpdateDecorator> SelectSource(IFlightAppDataQueryRepository flightAppData) => flightAppData.GetFlightsUpdate();

        public bool IsConditionMet(IFlightUpdateDecorator source) => true;

        public bool Update(IFlightUpdateDecorator source) => true;

        public bool Delete(IFlightUpdateDecorator source) => true;

        public CommandResult PrepareDisplayTable(IEnumerable<IFlightUpdateDecorator> source)
            => new CommandResult([$"{source.Count()} flights found"]);

        public CommandResult Add() => new CommandResult(["Flight added"]);
    }
}
