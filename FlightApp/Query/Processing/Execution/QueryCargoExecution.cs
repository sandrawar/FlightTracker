using FlightApp.DataProcessor;

namespace FlightApp.Query.Processing.Execution
{
    internal class QueryCargoExecution : IQueryExecution<ICargoUpdateDecorator>
    {
        private readonly FlighAppQueryData queryData;

        public QueryCargoExecution(FlighAppQueryData flighAppQueryData)
        {
            queryData = flighAppQueryData;
        }

        public IEnumerable<ICargoUpdateDecorator> SelectSource(IFlightAppDataQueryRepository flightAppData) => flightAppData.GetCargosUpdate();

        public bool IsConditionMet(ICargoUpdateDecorator source) => true;

        public bool Update(ICargoUpdateDecorator source) => true;

        public bool Delete(ICargoUpdateDecorator source) => true;

        public CommandResult PrepareDisplayTable(IEnumerable<ICargoUpdateDecorator> source)
            => new CommandResult([$"{source.Count()} cargos found"]);

        public CommandResult Add() => new CommandResult(["Cargo added"]);
    }
}
