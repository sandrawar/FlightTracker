using FlightApp.DataProcessor;

namespace FlightApp.Query.Processing.Execution
{
    internal class QueryAirportExecution : IQueryExecution<IAirportUpdateDecorator>
    {
        private readonly FlighAppQueryData queryData;

        public QueryAirportExecution(FlighAppQueryData flighAppQueryData)
        {
            queryData = flighAppQueryData;
        }

        public IEnumerable<IAirportUpdateDecorator> SelectSource(IFlightAppDataQueryRepository flightAppData) => flightAppData.GetAirportsUpdate();

        public bool IsConditionMet(IAirportUpdateDecorator source) => true;

        public bool Update(IAirportUpdateDecorator source) => true;

        public bool Delete(IAirportUpdateDecorator source) => true;

        public CommandResult PrepareDisplayTable(IEnumerable<IAirportUpdateDecorator> source)
            => new CommandResult([$"{source.Count()} airports found"]);

        public CommandResult Add() => new CommandResult(["Airport added"]);
    }
}
