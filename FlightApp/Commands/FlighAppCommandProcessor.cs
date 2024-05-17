using FlightApp.DataProcessor;
using FlightApp.Query;

namespace FlightApp.Commands
{
    internal interface IFlighAppCommandProcessor
    {
        CommandResult ProcessCommand(string command);
    }

    internal class FlighAppCommandProcessor : IFlighAppCommandProcessor
    {
        private IFlighAppCommand commandChain;

        public FlighAppCommandProcessor(IFlightAppCompleteData flightAppData, IFlighAppQueryProcessor flighAppQueryProcessor)
        {
            commandChain = new CommandChainBuilder()
                .InsertMakeSnapshot(flightAppData)
                .InsertGenerateNewsReport(flightAppData)
                .InsertDisplayQuery(flighAppQueryProcessor)
                .InsertAddQuery(flighAppQueryProcessor)
                .InsertDeleteQuery(flighAppQueryProcessor)
                .InsertUpdateQuery(flighAppQueryProcessor)
                .Build();
        }

        public CommandResult ProcessCommand(string command) => commandChain.Execute(command);
    }
}
