﻿using System.Text;

namespace FlightApp
{
    internal interface IFlightAppBinaryMessageReader
    {
        FlightAppObject Read(byte[] data);
    }

    internal class FlightAppBinaryMessageReader : IFlightAppBinaryMessageReader
    {
        private readonly IDictionary<string, IFlightAppBinaryMessageReader> readers;

        public FlightAppBinaryMessageReader()
        {
            readers = new Dictionary<string, IFlightAppBinaryMessageReader>()
            {
                 {"NCR", new CrewBinaryMessagReader() },
                 {"NPA", new PassangerBinaryMessagReader() },
                 {"NCA", new CargoBinaryMessagReader() },
                 {"NCP", new CargoPlaneBinaryMessagReader() },
                 {"NPP", new PassangerPlaneBinaryMessagReader() },
                 {"NAI", new AirportBinaryMessagReader() },
                 {"NFL", new FlightBinaryMessagReader() }
            };
        }

        public FlightAppObject Read(byte[] data)
        {
            var objectType = Encoding.ASCII.GetString(data, 0, 3);

            if (!readers.TryGetValue(objectType, out var reader))
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }
            return reader.Read(data);
        }
    }

    internal class CrewBinaryMessagReader : IFlightAppBinaryMessageReader
    {
        public FlightAppObject Read(byte[] data)
        {
            var nameLength = BitConverter.ToUInt16(data, 15);
            var emailLength = BitConverter.ToUInt16(data, 31 + nameLength);

            return new Crew(
                BitConverter.ToUInt64(data, 7),
                Encoding.ASCII.GetString(data, 17, nameLength),
                BitConverter.ToUInt16(data, 17 + nameLength),
                Encoding.ASCII.GetString(data, 19 + nameLength, 12),
                Encoding.ASCII.GetString(data, 33 + nameLength, emailLength),
                BitConverter.ToUInt16(data, 33 + nameLength + emailLength),
                Encoding.ASCII.GetString(data, 35 + nameLength + emailLength, 1));
        }
    }

    internal class PassangerBinaryMessagReader : IFlightAppBinaryMessageReader
    {
        public FlightAppObject Read(byte[] data)
        {
            var nameLength = BitConverter.ToUInt16(data, 15);
            var emailLength = BitConverter.ToUInt16(data, 31 + nameLength);

            return new Passanger(
                BitConverter.ToUInt64(data, 7),
                Encoding.ASCII.GetString(data, 17, nameLength),
                BitConverter.ToUInt16(data, 17 + nameLength),
                Encoding.ASCII.GetString(data, 19 + nameLength, 12),
                Encoding.ASCII.GetString(data, 33 + nameLength, emailLength),
                Encoding.ASCII.GetString(data, 33 + nameLength + emailLength, 1),
                BitConverter.ToUInt64(data, 34 + nameLength + emailLength));
        }
    }

    internal class CargoBinaryMessagReader : IFlightAppBinaryMessageReader
    {
        public FlightAppObject Read(byte[] data)
        {
            var descriptionLength = BitConverter.ToUInt16(data, 25);

            return new Cargo(
                BitConverter.ToUInt64(data, 7),
                BitConverter.ToSingle(data, 15),
                Encoding.ASCII.GetString(data, 19, 6),
                Encoding.ASCII.GetString(data, 27, descriptionLength));
        }
    }

    internal class CargoPlaneBinaryMessagReader : IFlightAppBinaryMessageReader
    {
        public FlightAppObject Read(byte[] data)
        {
            var modelLength = BitConverter.ToUInt16(data, 28);

            return new CargoPlane(
                BitConverter.ToUInt64(data, 7),
                Encoding.ASCII.GetString(data, 15, 10).TrimEnd('\0'),
                Encoding.ASCII.GetString(data, 25, 3),
                Encoding.ASCII.GetString(data, 30, modelLength),
                BitConverter.ToSingle(data, 30 + modelLength));
        }
    }

    internal class PassangerPlaneBinaryMessagReader : IFlightAppBinaryMessageReader
    {
        public FlightAppObject Read(byte[] data)
        {
            var modelLength = BitConverter.ToUInt16(data, 28);

            return new PassangerPlane(
                BitConverter.ToUInt64(data, 7),
                Encoding.ASCII.GetString(data, 15, 10).TrimEnd('\0'),
                Encoding.ASCII.GetString(data, 25, 3),
                Encoding.ASCII.GetString(data, 30, modelLength),
                BitConverter.ToUInt16(data, 30 + modelLength),
                BitConverter.ToUInt16(data, 32 + modelLength),
                BitConverter.ToUInt16(data, 34 + modelLength));
        }
    }

    internal class AirportBinaryMessagReader : IFlightAppBinaryMessageReader
    {
        public FlightAppObject Read(byte[] data)
        {
            var nameLength = BitConverter.ToUInt16(data, 15);

            return new Airport(
                BitConverter.ToUInt64(data, 7),
                Encoding.ASCII.GetString(data, 17, nameLength),
                Encoding.ASCII.GetString(data, 17 + nameLength, 3),
                BitConverter.ToSingle(data, 20 + nameLength),
                BitConverter.ToSingle(data, 24 + nameLength),
                BitConverter.ToSingle(data, 28 + nameLength),
                Encoding.ASCII.GetString(data, 32 + nameLength, 3));
        }
    }

    internal class FlightBinaryMessagReader : IFlightAppBinaryMessageReader
    {
        public FlightAppObject Read(byte[] data)
        {
            var crewCount = BitConverter.ToUInt16(data, 55);
            var loadCount = BitConverter.ToUInt16(data, 57 + 8 * crewCount);

            return new Flight(
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
                );

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
