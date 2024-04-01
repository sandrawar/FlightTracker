using FlightApp.Readers;
using NetworkSourceSimulator;

namespace FlightApp.DataProcessor
{
    internal class NetworkSimulatorDataProcessor : IFlightAppDataProcessor
    {
        private IFlightAppCompleteData? flightAppCompleteData;
        private readonly IFlightAppBinaryMessageReader messageReader;
        private readonly IDataSource simulator;

        public NetworkSimulatorDataProcessor(string dataFilePath)
        {
            this.messageReader = new FlightAppBinaryMessageReader();
            this.simulator = new NetworkSourceSimulator.NetworkSourceSimulator(dataFilePath, 1, 5);
            this.simulator.OnNewDataReady += Simulator_OnNewDataReady;
        }

        public void Start(IFlightAppCompleteData flightAppCompleteData)
        {
            this.flightAppCompleteData = flightAppCompleteData;
            Thread simulateThread = new Thread(() => simulator.Run());
            simulateThread.IsBackground = true;
            simulateThread.Start();
        }

        private void Simulator_OnNewDataReady(object sender, NewDataReadyArgs args)
        {
            var message = simulator.GetMessageAt(args.MessageIndex);
            messageReader.AddToFlightAppCompleteData(message.MessageBytes, flightAppCompleteData!);
        }

    }
}
