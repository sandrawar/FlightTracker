using FlightApp.Query;
using System.Text.RegularExpressions;

namespace FlightApp.Commands
{
    internal class DisplayQuery : CommandChainQuery
    {
        private Regex regex;

        public DisplayQuery(IFlighAppQueryProcessor queryProcessor, IFlighAppCommand nextCommandInChain) : base(queryProcessor, nextCommandInChain)
        {
            regex = new Regex(@"(?<operation>display)\s+(?<fields>\*|[\w, ]+)\s+from\s+(?<class>\w+)(\s+where\s+(?<conditions>.+))?", RegexOptions.Compiled);
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
                    null,
                    match.Groups["fields"]?.Value
                ));
        }
    }
}
