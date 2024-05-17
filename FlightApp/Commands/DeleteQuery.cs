using FlightApp.Query;
using System.Text.RegularExpressions;

namespace FlightApp.Commands
{
    internal class DeleteQuery : CommandChainQuery
    {
        private Regex regex;
     
        public DeleteQuery(IFlighAppQueryProcessor queryProcessor, IFlighAppCommand nextCommandInChain) : base(queryProcessor, nextCommandInChain)
        {
            regex = new Regex(@"(?<operation>delete)\s+(?<class>\w+)(?:\s+where\s+(?<conditions>.+))?", RegexOptions.Compiled);
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
                    null
                ));
        }
    }
}
