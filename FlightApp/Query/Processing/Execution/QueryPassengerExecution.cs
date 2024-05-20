using FlightApp.DataProcessor;

namespace FlightApp.Query.Processing.Execution
{
    internal class QueryPassengerExecution : IQueryExecution<IPassangerUpdateDecorator>
    {
        private readonly FlighAppQueryData queryData;

        public QueryPassengerExecution(FlighAppQueryData flighAppQueryData)
        {
            queryData = flighAppQueryData;
        }

        public IEnumerable<IPassangerUpdateDecorator> SelectSource(IFlightAppDataQueryRepository flightAppData) => flightAppData.GetPassangersUpdate();

        public bool IsConditionMet(IPassangerUpdateDecorator source) => true;

        public bool Update(IPassangerUpdateDecorator source) => true;

        public bool Delete(IPassangerUpdateDecorator source) => true;

        public CommandResult PrepareDisplayTable(IEnumerable<IPassangerUpdateDecorator> source)
            => new CommandResult([$"{source.Count()} passangers found"]);

        public CommandResult Add() => new CommandResult(["Passanger added"]);
    }
}
