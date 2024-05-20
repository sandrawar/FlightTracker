using FlightApp.Query;

namespace FlightApp.Command.Operation
{
    internal class QueryCommandOperation : CommandOperationBase
    {
        private readonly IFlightAppQueryParser parser;
        private readonly IFlighAppQueryLibrary library;

        public QueryCommandOperation(IFlightAppQueryParser queryParser, IFlighAppQueryLibrary queryLibrary, IFlighAppCommand nextCommandInChain) : base(nextCommandInChain)
        {
            parser = queryParser;
            library = queryLibrary;
        }

        protected override CommandResult? ExecuteCommand(string command)
        {
            if (!parser.IsQueryCommand(command))
            {
                return null;
            }

            var query = parser.Parse(command);
            if (query is null)
            {
                return new CommandResult(["query parse error"]);
            }

            return library.Execute(query);
        }
    }
}
