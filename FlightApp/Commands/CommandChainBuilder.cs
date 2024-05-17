using FlightApp.DataProcessor;
using FlightApp.Query;

namespace FlightApp.Commands
{
    internal class CommandChainBuilder
    {
        private IFlighAppCommand commandChainStart;
        
        public CommandChainBuilder()
        {
            commandChainStart = ResetChain();
        }

        public void Reset() => ResetChain();

        public CommandChainBuilder InsertMakeSnapshot(IFlightAppDataRead data)
        {
            commandChainStart = new MakeSnapshot(data, commandChainStart);
            return this;
        }

        public CommandChainBuilder InsertGenerateNewsReport(IFlightAppDataRead data)
        {
            commandChainStart = new GenerateNewsReport(data, commandChainStart);
            return this;
        }

        public CommandChainBuilder InsertDisplayQuery(IFlighAppQueryProcessor queryProcessor)
        {
            commandChainStart = new DisplayQuery(queryProcessor, commandChainStart);
            return this;
        }

        public CommandChainBuilder InsertAddQuery(IFlighAppQueryProcessor queryProcessor)
        {
            commandChainStart = new AddQuery(queryProcessor, commandChainStart);
            return this;
        }

        public CommandChainBuilder InsertDeleteQuery(IFlighAppQueryProcessor queryProcessor)
        {
            commandChainStart = new DeleteQuery(queryProcessor, commandChainStart);
            return this;
        }

        public CommandChainBuilder InsertUpdateQuery(IFlighAppQueryProcessor queryProcessor)
        {
            commandChainStart = new UpdateQuery(queryProcessor, commandChainStart);
            return this;
        }

        public IFlighAppCommand Build()
        {
            var chainStart = commandChainStart;
            ResetChain();
            return chainStart;
        }

        private IFlighAppCommand ResetChain()
        {
            commandChainStart = new CommandChainTermination();
            return commandChainStart;

        }
    }
}
