using FlightApp.DataProcessor;
using System.Text.RegularExpressions;

namespace FlightApp.Commands
{
    internal class DeleteQuery : CommandChainUpdate
    {
        private Regex regex;
     
        public DeleteQuery(IFlightAppDataUpdate data, IFlighAppCommand nextCommandInChain) : base(data, nextCommandInChain)
        {
            regex = new Regex(@"delete\s+(\w+)(?:\s+where\s+(.+))", RegexOptions.Compiled);
        }

        protected override CommandResult? ExecuteCommand(string commands)
        {
            if (!regex.IsMatch(commands))
            {
                return null;
            }

            return new CommandResult([$"delete: {commands}"]);
        }
    }
}
