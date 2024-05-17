using FlightApp.DataProcessor;
using System.Text.RegularExpressions;

namespace FlightApp.Commands
{
    internal class UpdateQuery : CommandChainUpdate
    {
        private Regex regex;

        public UpdateQuery(IFlightAppDataUpdate data, IFlighAppCommand nextCommandInChain) : base(data, nextCommandInChain)
        {
            regex = new Regex(@"update\s+(\w+)\s+set\s+\((.+)\)(?:\s+where\s+(.+))", RegexOptions.Compiled);
        }

        protected override CommandResult? ExecuteCommand(string commands)
        {
            if (!regex.IsMatch(commands))
            {
                return null;
            }

            return new CommandResult([$"update: {commands}"]);
        }
    }
}
