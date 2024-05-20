using FlightApp.Chain;
using FlightApp.Command.Operation;
using FlightApp.DataProcessor;
using FlightApp.Query;

namespace FlightApp.Command
{
    internal class FlighAppCommandChain : Chain<IFlighAppCommand>
    {
        public FlighAppCommandChain() : base(() => new CommandChainTermination(["Unrecognized command"]))
        {
        }

        public FlighAppCommandChain MakeSnapshot(IFlightAppDataRead data)
        {
            AddLink(next => new MakeSnapshotCommandOperation(data, next));
            return this;
        }

        public FlighAppCommandChain GenerateNewsReport(IFlightAppDataRead data)
        {
            AddLink(next => new GenerateNewsReportCommandOperation(data, next));
            return this;
        }

        public FlighAppCommandChain Query(IFlightAppQueryParser queryParser, IFlighAppQueryLibrary queryProcessor)
        {
            AddLink(next => new QueryCommandOperation(queryParser, queryProcessor, next));
            return this;
        }
    }
}
