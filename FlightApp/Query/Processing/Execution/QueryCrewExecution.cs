using FlightApp.DataProcessor;

namespace FlightApp.Query.Processing.Execution
{
    internal class QueryCrewExecution : IQueryExecution<ICrewUpdateDecorator>
    {
        private readonly FlighAppQueryData queryData;

        public QueryCrewExecution(FlighAppQueryData flighAppQueryData)
        {
            queryData = flighAppQueryData;
        }

        public IEnumerable<ICrewUpdateDecorator> SelectSource(IFlightAppDataQueryRepository flightAppData) => flightAppData.GetCrewMembersUpdate();

        public bool IsConditionMet(ICrewUpdateDecorator source) => true;

        public bool Update(ICrewUpdateDecorator source) => true;

        public bool Delete(ICrewUpdateDecorator source) => true;

        public CommandResult PrepareDisplayTable(IEnumerable<ICrewUpdateDecorator> source)
            => new CommandResult([$"{source.Count()} crew found"]);

        public CommandResult Add() => new CommandResult(["Crew added"]);
    }
}
