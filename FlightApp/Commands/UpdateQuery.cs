using FlightApp.Query;
using System.Text.RegularExpressions;

namespace FlightApp.Commands
{
    internal class UpdateQuery : CommandChainQuery
    {
        private Regex regex;

        public UpdateQuery(IFlighAppQueryProcessor queryProcessor, IFlighAppCommand nextCommandInChain) : base(queryProcessor, nextCommandInChain)
        {
            regex = new Regex(@"(?<operation>update)\s+(?<class>\w+)\s+set\s+\((?<values>.+)\)(?:\s+where\s+(?<conditions>.+))?", RegexOptions.Compiled);
        }

        protected override CommandResult? ExecuteCommand(string commands)
        {
            var match = regex.Match(commands);
            if (!match.Success) 
            {
                return null;
            }

            return QueryProcessor.ExecuteQuery(
                new FlighAppQuery(
                    match.Groups["operation"].Value,
                    match.Groups["class"].Value,
                    match.Groups["conditions"]?.Value,
                    match.Groups["values"].Value,
                    null
                ));
        }
    }
}
