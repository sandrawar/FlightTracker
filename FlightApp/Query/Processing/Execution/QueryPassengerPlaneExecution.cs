using FlightApp.DataProcessor;

namespace FlightApp.Query.Processing.Execution
{
    internal class QueryPassengerPlaneExecution : IQueryExecution<IPassangerPlaneUpdateDecorator>
    {
        private readonly FlighAppQueryData queryData;

        public QueryPassengerPlaneExecution(FlighAppQueryData flighAppQueryData)
        {
            queryData = flighAppQueryData;
        }

        public IEnumerable<IPassangerPlaneUpdateDecorator> SelectSource(IFlightAppDataQueryRepository flightAppData) => flightAppData.GetPassangerPlanesUpdate();

        public bool IsConditionMet(IPassangerPlaneUpdateDecorator source) => true;

        public bool Update(IPassangerPlaneUpdateDecorator source) => true;

        public bool Delete(IPassangerPlaneUpdateDecorator source) => true;

        public CommandResult PrepareDisplayTable(IEnumerable<IPassangerPlaneUpdateDecorator> source)
            => new CommandResult([$"{source.Count()} passanger planes found"]);

        public CommandResult Add() => new CommandResult(["Passanger plane added"]);
    }
}
