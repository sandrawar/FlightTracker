using System.Text;
using FlightApp.DataProcessor;

namespace FlightApp.Readers
{
    internal interface IFlightAppBinaryMessageReader : IFlightAppObjectReader<byte[]>
    {
    }

    internal class FlightAppBinaryMessageReader : IFlightAppBinaryMessageReader
    {
        private readonly IDictionary<string, IFlightAppBinaryMessageReader> readers;

        public FlightAppBinaryMessageReader()
        {
            readers = new Dictionary<string, IFlightAppBinaryMessageReader>()
            {
                 {"NCR", new CrewBinaryMessagReader() },
                 {"NPA", new PassengerBinaryMessagReader() },
                 {"NCA", new CargoBinaryMessagReader() },
                 {"NCP", new CargoPlaneBinaryMessagReader() },
                 {"NPP", new PassengerPlaneBinaryMessagReader() },
                 {"NAI", new AirportBinaryMessagReader() },
                 {"NFL", new FlightBinaryMessagReader() }
            };
        }

        public void AddToFlightAppDataUpdate(byte[] data, IFlightAppDataUpdate flightAppDataUpdate)
        {
            var objectType = Encoding.ASCII.GetString(data, 0, 3);

            if (!readers.TryGetValue(objectType, out var reader))
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }
            reader.AddToFlightAppDataUpdate(data, flightAppDataUpdate);
        }
    }

    internal class CrewBinaryMessagReader : IFlightAppBinaryMessageReader
    {
        public void AddToFlightAppDataUpdate(byte[] data, IFlightAppDataUpdate flightAppDataUpdate)
        {
            var nameLength = BitConverter.ToUInt16(data, 15);
            var emailLength = BitConverter.ToUInt16(data, 31 + nameLength);

            flightAppDataUpdate.Add(new Crew(
                BitConverter.ToUInt64(data, 7),
                Encoding.ASCII.GetString(data, 17, nameLength),
                BitConverter.ToUInt16(data, 17 + nameLength),
                Encoding.ASCII.GetString(data, 19 + nameLength, 12),
                Encoding.ASCII.GetString(data, 33 + nameLength, emailLength),
                BitConverter.ToUInt16(data, 33 + nameLength + emailLength),
                GetRole(Encoding.ASCII.GetString(data, 35 + nameLength + emailLength, 1))));

            string GetRole(string roleId) => roleId switch
            {
                "C" => "Captain",
                "A" => "Attendant",
                "O" => "Other",
                _ => "Other"
            };
        }
    }

    internal class PassengerBinaryMessagReader : IFlightAppBinaryMessageReader
    {
        public void AddToFlightAppDataUpdate(byte[] data, IFlightAppDataUpdate flightAppDataUpdate)
        {
            var nameLength = BitConverter.ToUInt16(data, 15);
            var emailLength = BitConverter.ToUInt16(data, 31 + nameLength);

            flightAppDataUpdate.Add(new Passenger(
                BitConverter.ToUInt64(data, 7),
                Encoding.ASCII.GetString(data, 17, nameLength),
                BitConverter.ToUInt16(data, 17 + nameLength),
                Encoding.ASCII.GetString(data, 19 + nameLength, 12),
                Encoding.ASCII.GetString(data, 33 + nameLength, emailLength),
                Encoding.ASCII.GetString(data, 33 + nameLength + emailLength, 1),
                BitConverter.ToUInt64(data, 34 + nameLength + emailLength)));
        }
    }

    internal class CargoBinaryMessagReader : IFlightAppBinaryMessageReader
    {
        public void AddToFlightAppDataUpdate(byte[] data, IFlightAppDataUpdate flightAppDataUpdate)
        {
            var descriptionLength = BitConverter.ToUInt16(data, 25);

            flightAppDataUpdate.Add(new Cargo(
                BitConverter.ToUInt64(data, 7),
                BitConverter.ToSingle(data, 15),
                Encoding.ASCII.GetString(data, 19, 6),
                Encoding.ASCII.GetString(data, 27, descriptionLength)));
        }
    }

    internal class CargoPlaneBinaryMessagReader : IFlightAppBinaryMessageReader
    {
        public void AddToFlightAppDataUpdate(byte[] data, IFlightAppDataUpdate flightAppDataUpdate)
        {
            var modelLength = BitConverter.ToUInt16(data, 28);

            flightAppDataUpdate.Add(new CargoPlane(
                BitConverter.ToUInt64(data, 7),
                Encoding.ASCII.GetString(data, 15, 10).TrimEnd('\0'),
                Encoding.ASCII.GetString(data, 25, 3),
                Encoding.ASCII.GetString(data, 30, modelLength),
                BitConverter.ToSingle(data, 30 + modelLength)));
        }
    }

    internal class PassengerPlaneBinaryMessagReader : IFlightAppBinaryMessageReader
    {
        public void AddToFlightAppDataUpdate(byte[] data, IFlightAppDataUpdate flightAppDataUpdate)
        {
            var modelLength = BitConverter.ToUInt16(data, 28);

            flightAppDataUpdate.Add(new PassengerPlane(
                BitConverter.ToUInt64(data, 7),
                Encoding.ASCII.GetString(data, 15, 10).TrimEnd('\0'),
                Encoding.ASCII.GetString(data, 25, 3),
                Encoding.ASCII.GetString(data, 30, modelLength),
                BitConverter.ToUInt16(data, 30 + modelLength),
                BitConverter.ToUInt16(data, 32 + modelLength),
                BitConverter.ToUInt16(data, 34 + modelLength)));
        }
    }

    internal class AirportBinaryMessagReader : IFlightAppBinaryMessageReader
    {
        public void AddToFlightAppDataUpdate(byte[] data, IFlightAppDataUpdate flightAppDataUpdate)
        {
            var nameLength = BitConverter.ToUInt16(data, 15);

            flightAppDataUpdate.Add(new Airport(
                BitConverter.ToUInt64(data, 7),
                Encoding.ASCII.GetString(data, 17, nameLength),
                Encoding.ASCII.GetString(data, 17 + nameLength, 3),
                BitConverter.ToSingle(data, 20 + nameLength),
                BitConverter.ToSingle(data, 24 + nameLength),
                BitConverter.ToSingle(data, 28 + nameLength),
                Encoding.ASCII.GetString(data, 32 + nameLength, 3)));
        }
    }

    internal class FlightBinaryMessagReader : IFlightAppBinaryMessageReader
    {
        public void AddToFlightAppDataUpdate(byte[] data, IFlightAppDataUpdate flightAppDataUpdate)
        {
            var crewCount = BitConverter.ToUInt16(data, 55);
            var loadCount = BitConverter.ToUInt16(data, 57 + 8 * crewCount);

            flightAppDataUpdate.Add(new Flight(
                BitConverter.ToUInt64(data, 7),
                BitConverter.ToUInt64(data, 15),
                BitConverter.ToUInt64(data, 23),
                DateTime.UnixEpoch.AddMilliseconds(BitConverter.ToUInt64(data, 31)),
                DateTime.UnixEpoch.AddMilliseconds(BitConverter.ToUInt64(data, 39)),
                null,
                null,
                null,
                BitConverter.ToUInt64(data, 47),
                readIds(57, crewCount),
                readIds(57 + 8 * crewCount, loadCount)
                ));

            ulong[] readIds(int offset, ushort count)
            {
                var ids = new List<ulong>(count);
                for (var i = 0; i < count; i++)
                {
                    var id = BitConverter.ToUInt64(data, offset + 8 * i);
                    ids.Add(id);
                }
                return ids.ToArray();
            }
        }
    }
}
