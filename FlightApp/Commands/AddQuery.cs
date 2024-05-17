using FlightApp.DataProcessor;
using System.Text.RegularExpressions;

namespace FlightApp.Commands
{
    internal class AddQuery : CommandChainUpdate
    {
        private Regex regex;

        public AddQuery(IFlightAppDataUpdate data, IFlighAppCommand nextCommandInChain) : base(data, nextCommandInChain)
        {
            regex = new Regex(@"add\s+(\w+)\s+new\s+\((.+)\)", RegexOptions.Compiled);
        }

        protected override CommandResult? ExecuteCommand(string commands)
        {
            if (!regex.IsMatch(commands))
            {
                return null;
            }

            return new CommandResult([$"add: {commands}"]);
        }
    }
}
