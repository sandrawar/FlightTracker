using FlightApp.Readers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightApp.DataProcessor
{
    internal sealed class DataProcessorFactory
    {
        private enum DataProcessorKind
        {
            SimpleFtr,
            NetworkSimulator,
            FtrSimulatorUnion
        }

        private static DataProcessorKind dataProcessorKind = DataProcessorKind.FtrSimulatorUnion;

        public IFlightAppDataProcessor Create(string dataFilePath, string dataUpdateFilePath, IFlightAppDataUpdate flightAppDataUpdate)
            => dataProcessorKind switch
            {
                DataProcessorKind.SimpleFtr => new FtrDataProcessor(
                    dataFilePath,
                    new FlightAppFtrReader(),
                    flightAppDataUpdate),

                DataProcessorKind.NetworkSimulator => new NetworkSimulatorDataProcessor(
                    dataFilePath,
                    new FlightAppBinaryMessageReader(),
                    flightAppDataUpdate),

                DataProcessorKind.FtrSimulatorUnion => new UnionDataProcessor(
                    [
                        new FtrDataProcessor(dataFilePath, new FlightAppFtrReader(), flightAppDataUpdate),
                        new NetworkSimulatorDataProcessor(dataUpdateFilePath, new FlightAppBinaryMessageReader(), flightAppDataUpdate)
                    ]),
                _ => throw new NotImplementedException(),
            };
    }
}
