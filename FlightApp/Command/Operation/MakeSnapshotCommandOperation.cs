using FlightApp.DataProcessor;
using FlightApp.Serializer;

namespace FlightApp.Command.Operation
{
    internal class MakeSnapshotCommandOperation : CommandChainRead
    {
        public MakeSnapshotCommandOperation(IFlightAppDataRead data, IFlighAppCommand nextCommandInChain) : base(data, nextCommandInChain)
        {
        }

        protected override CommandResult? ExecuteCommand(string command)
        {
            if (command != "print")
            {
                return null;
            }

            var snapshotFileName = $"snapshot_{DateTime.Now:HH_mm_ss}.json";
            IDataSerializer serializer = new DataJsonSerializer();
            var data = Data.GetCompleteData();
            var fileName = serializer.Serialize(data, snapshotFileName);

            return new CommandResult([$"Snapshot generated: {fileName}"]);
        }
    }
}
