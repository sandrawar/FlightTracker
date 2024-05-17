using FlightApp.DataProcessor;

namespace FlightApp.Commands
{
    internal interface IFlighAppCommand
    {
        CommandResult Execute(string command);
    }

    internal abstract class FlighAppCommandChainBase : IFlighAppCommand
    {
        private readonly IFlighAppCommand nextCommandInChain;

        protected FlighAppCommandChainBase(IFlighAppCommand nextCommandInChain)
        {
            this.nextCommandInChain = nextCommandInChain;
        }

        public CommandResult Execute(string command)
            => ExecuteCommand(command) ?? nextCommandInChain.Execute(command);

        protected abstract CommandResult? ExecuteCommand(string commands);
    }


    internal abstract class CommandChainRead : FlighAppCommandChainBase
    {
        protected CommandChainRead(IFlightAppDataRead data, IFlighAppCommand nextCommandInChain): base(nextCommandInChain)
        {
            Data = data;
        }

        protected IFlightAppDataRead Data { get; }
    }

    internal abstract class CommandChainUpdate : FlighAppCommandChainBase
    {
        protected CommandChainUpdate(IFlightAppDataUpdate data, IFlighAppCommand nextCommandInChain) : base(nextCommandInChain)
        {
            Data = data;
        }

        protected IFlightAppDataUpdate Data { get; }
    }

    internal class CommandChainTermination : IFlighAppCommand
    {
        public CommandResult Execute(string? command) => new CommandResult(["Unrecognized command"]);
    }
}
