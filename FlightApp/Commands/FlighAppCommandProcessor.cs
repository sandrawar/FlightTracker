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

    internal class CommandChainBuilder
    {
        private IFlighAppCommand commandChainStart;
        
        public CommandChainBuilder()
        {
            commandChainStart = ResetChain();
        }

        public void Reset() => ResetChain();

        public void InsertCommand(Func<IFlighAppCommand, IFlighAppCommand> commandBuild)
        {
            commandChainStart = commandBuild(commandChainStart);
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

    internal static class CommandChainBuilderExtensions
    {
        public static CommandChainBuilder InsertMakeSnapshot(this CommandChainBuilder builder, IFlightAppDataRead data)
        {
            builder.InsertCommand(nextInChain => new MakeSnapshot(data, nextInChain));
            return builder;
        }

        public static CommandChainBuilder InsertGenerateNewsReport(this CommandChainBuilder builder, IFlightAppDataRead data)
        {
            builder.InsertCommand(nextInChain => new GenerateNewsReport(data, nextInChain));
            return builder;
        }

        public static CommandChainBuilder InsertDisplayQuery(this CommandChainBuilder builder, IFlighAppQueryProcessor queryProcessor)
        {
            builder.InsertCommand(nextInChain => new DisplayQuery(queryProcessor, nextInChain));
            return builder;
        }

        public static CommandChainBuilder InsertAddQuery(this CommandChainBuilder builder, IFlighAppQueryProcessor queryProcessor)
        {
            builder.InsertCommand(nextInChain => new AddQuery(queryProcessor, nextInChain));
            return builder;
        }

        public static CommandChainBuilder InsertDeleteQuery(this CommandChainBuilder builder, IFlighAppQueryProcessor queryProcessor)
        {
            builder.InsertCommand(nextInChain => new DeleteQuery(queryProcessor, nextInChain));
            return builder;
        }

        public static CommandChainBuilder InsertUpdateQuery(this CommandChainBuilder builder, IFlighAppQueryProcessor queryProcessor)
        {
            builder.InsertCommand(nextInChain => new UpdateQuery(queryProcessor, nextInChain));
            return builder;
        }
    }
}
