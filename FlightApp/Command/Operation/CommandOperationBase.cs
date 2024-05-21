using FlightApp.DataProcessor;

namespace FlightApp.Command.Operation
{

    internal abstract class CommandOperationBase : IFlighAppCommand
    {
        private readonly IFlighAppCommand nextCommandInChain;

        protected CommandOperationBase(IFlighAppCommand nextCommand)
        {
            nextCommandInChain = nextCommand;
        }

        public CommandResult Execute(string command)
            => ExecuteCommand(command) ?? nextCommandInChain.Execute(command);

        protected abstract CommandResult? ExecuteCommand(string command);
    }

    internal abstract class CommandChainRead : CommandOperationBase
    {
        protected CommandChainRead(IFlightAppDataRead data, IFlighAppCommand nextCommandInChain) : base(nextCommandInChain)
        {
            Data = data;
        }

        protected IFlightAppDataRead Data { get; }
    }

    internal class CommandChainTermination : IFlighAppCommand
    {
        private readonly IReadOnlyCollection<string> messages;

        public CommandChainTermination(IReadOnlyCollection<string> resultMessages)
        {
            messages = resultMessages;
        }

        public CommandResult Execute(string? command) => new CommandResult(messages);
    }
}
