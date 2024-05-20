using FlightApp.DataProcessor;
using FlightApp.Query;

namespace FlightApp.Command
{
    internal interface IFlighAppCommandLibrary : IFlighAppCommand
    {
    }

    internal class FlighAppCommandLibrary : IFlighAppCommandLibrary
    {
        private IFlighAppCommand commandChain;

        public FlighAppCommandLibrary(IFlightAppCompleteData flightAppData, IFlightAppQueryParser flighAppQueryParser, IFlighAppQueryLibrary flighAppQueryProcessor)
        {
            commandChain = new FlighAppCommandChain()
                .MakeSnapshot(flightAppData)
                .GenerateNewsReport(flightAppData)
                .Query(flighAppQueryParser, flighAppQueryProcessor)
                .Build();
        }

        public CommandResult Execute(string command) => commandChain.Execute(command);
    }
}
