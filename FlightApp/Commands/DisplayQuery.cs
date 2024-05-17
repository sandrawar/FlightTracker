using FlightApp.DataProcessor;
using System.Text.RegularExpressions;

namespace FlightApp.Commands
{
    internal class DisplayQuery : CommandChainRead
    {
        private Regex regex;

        public DisplayQuery(IFlightAppDataRead data, IFlighAppCommand nextCommandInChain) : base(data, nextCommandInChain)
        {
            regex = new Regex(@"display\s+(?<fields>\*|[\w, ]+)\s+from\s+(?<class>\w+)(\s+where\s+(?<conditions>.+))", RegexOptions.Compiled);
        }

        protected override CommandResult? ExecuteCommand(string commands)
        {
            if (!regex.IsMatch(commands))
            {
                return null;
            }

            return new CommandResult([$"display: {commands}"]);
        }
    }
}
