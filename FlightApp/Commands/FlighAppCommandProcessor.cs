using FlightApp.DataProcessor;

namespace FlightApp.Commands
{
    internal interface IFlighAppCommandProcessor
    {
        CommandResult ProcessCommand(string command);
    }

    internal class FlighAppCommandProcessor : IFlighAppCommandProcessor
    {
        private IFlighAppCommand commandChain;

        public FlighAppCommandProcessor(IFlightAppCompleteData flightAppData)
        {
            commandChain = new CommandChainBuilder()
                .InsertMakeSnapshot(flightAppData)
                .InsertGenerateNewsReport(flightAppData)
                .InsertDisplayQuery(flightAppData)
                .InsertAddQuery(flightAppData)
                .InsertDeleteQuery(flightAppData)
                .InsertUpdateQuery(flightAppData)
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

        public static CommandChainBuilder InsertDisplayQuery(this CommandChainBuilder builder, IFlightAppDataRead data)
        {
            builder.InsertCommand(nextInChain => new DisplayQuery(data, nextInChain));
            return builder;
        }

        public static CommandChainBuilder InsertAddQuery(this CommandChainBuilder builder, IFlightAppDataUpdate data)
        {
            builder.InsertCommand(nextInChain => new AddQuery(data, nextInChain));
            return builder;
        }

        public static CommandChainBuilder InsertDeleteQuery(this CommandChainBuilder builder, IFlightAppDataUpdate data)
        {
            builder.InsertCommand(nextInChain => new DeleteQuery(data, nextInChain));
            return builder;
        }

        public static CommandChainBuilder InsertUpdateQuery(this CommandChainBuilder builder, IFlightAppDataUpdate data)
        {
            builder.InsertCommand(nextInChain => new UpdateQuery(data, nextInChain));
            return builder;
        }
    }
}
