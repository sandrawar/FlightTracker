using FlightApp.Query;
using System.Text.RegularExpressions;

namespace FlightApp.Commands
{
    internal class AddQuery : CommandChainQuery
    {
        private Regex regex;

        public AddQuery(IFlighAppQueryProcessor queryProcessor, IFlighAppCommand nextCommandInChain) : base(queryProcessor, nextCommandInChain)
        {
            regex = new Regex(@"(?<operation>add)\s+(?<class>\w+)\s+new\s+\((?<values>.+)\)", RegexOptions.Compiled);
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
                    null,
                    match.Groups["values"].Value,
                    null
                ));
        }
    }
}
