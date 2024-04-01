using FlightApp.Readers;
using NetworkSourceSimulator;

namespace FlightApp.DataProcessor
{
    internal class NetworkSimulatorDataProcessor : IFlightAppDataProcessor
    {
        private readonly IFlightAppBinaryMessageReader messageReader;
        private readonly IDataSource simulator;

        public NetworkSimulatorDataProcessor(string dataFilePath, IFlightAppBinaryMessageReader flightAppBinaryMessageReader, IFlightAppCompleteData flightAppCompleteData)
        {
            FlightAppCompleteData = flightAppCompleteData;
            messageReader = flightAppBinaryMessageReader;
            simulator = new NetworkSourceSimulator.NetworkSourceSimulator(dataFilePath, 1, 5);
            simulator.OnNewDataReady += Simulator_OnNewDataReady;
        }

        public IFlightAppCompleteData FlightAppCompleteData { get; }

        public void Start()
        {
            Thread simulateThread = new Thread(() => simulator.Run());
            simulateThread.IsBackground = true;
            simulateThread.Start();
        }

        private void Simulator_OnNewDataReady(object sender, NewDataReadyArgs args)
        {
            var message = simulator.GetMessageAt(args.MessageIndex);
            messageReader.AddToFlightAppCompleteData(message.MessageBytes, FlightAppCompleteData);
        }

    }
}
