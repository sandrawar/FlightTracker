using FlightApp.Readers;
using NetworkSourceSimulator;

namespace FlightApp.DataProcessor
{
    internal class NetworkSimulatorDataProcessor : IFlightAppDataProcessor
    {
        private readonly IFlightAppBinaryMessageReader messageReader;
        private readonly IFlightAppDataUpdate flightAppData;
        private readonly NetworkSourceSimulator.NetworkSourceSimulator simulator;

        public NetworkSimulatorDataProcessor(string dataFilePath, IFlightAppBinaryMessageReader flightAppBinaryMessageReader, IFlightAppDataUpdate flightAppDataUpdate)
        {
            flightAppData = flightAppDataUpdate;
            messageReader = flightAppBinaryMessageReader;
            simulator = new NetworkSourceSimulator.NetworkSourceSimulator(dataFilePath, 1, 5);
            simulator.OnNewDataReady += Simulator_OnNewDataReady;
            simulator.OnContactInfoUpdate += Simulator_OnContactInfoUpdate;
            simulator.OnIDUpdate += Simulator_OnIDUpdate;
            simulator.OnPositionUpdate += Simulator_OnPositionUpdate;
        }

        public void Start()
        {
            Thread simulateThread = new Thread(() => simulator.Run());
            simulateThread.IsBackground = true;
            simulateThread.Start();
        }

        private void Simulator_OnNewDataReady(object sender, NewDataReadyArgs args)
        {
            var message = simulator.GetMessageAt(args.MessageIndex);
            messageReader.AddToFlightAppDataUpdate(message.MessageBytes, flightAppData);
        }

        private void Simulator_OnPositionUpdate(object sender, PositionUpdateArgs args)
            => flightAppData.UpdateData(new PositionUpdateData(args.ObjectID, args.Longitude, args.Latitude, args.AMSL));

        private void Simulator_OnIDUpdate(object sender, IDUpdateArgs args)
            => flightAppData.UpdateData(new IDUpdateData(args.ObjectID, args.NewObjectID));

        private void Simulator_OnContactInfoUpdate(object sender, ContactInfoUpdateArgs args)
            => flightAppData.UpdateData(new ContactInfoUpdateData(args.ObjectID, args.PhoneNumber, args.EmailAddress));

    }
}
