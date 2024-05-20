using FlightApp.DataProcessor;

namespace FlightApp.Query.Processing.Execution
{
    internal class QueryCargoPlaneExecution : IQueryExecution<ICargoPlaneUpdateDecorator>
    {
        private readonly FlighAppQueryData queryData;

        public QueryCargoPlaneExecution(FlighAppQueryData flighAppQueryData)
        {
            queryData = flighAppQueryData;
        }

        public IEnumerable<ICargoPlaneUpdateDecorator> SelectSource(IFlightAppDataQueryRepository flightAppData) => flightAppData.GetCargoPlanesUpdate();

        public bool IsConditionMet(ICargoPlaneUpdateDecorator source) => true;

        public bool Update(ICargoPlaneUpdateDecorator source) => true;

        public bool Delete(ICargoPlaneUpdateDecorator source) => true;

        public CommandResult PrepareDisplayTable(IEnumerable<ICargoPlaneUpdateDecorator> source)
            => new CommandResult([$"{source.Count()} cargo planes found"]);

        public CommandResult Add() => new CommandResult(["Cargo plane added"]);
    }
}
