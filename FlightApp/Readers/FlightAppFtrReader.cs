using System.Data;
using System.Globalization;
using FlightApp.DataProcessor;

namespace FlightApp.Readers;

internal interface IFlightAppObjectFtrReader : IFlightAppObjectReader<string[]>
{
}

internal class FlightAppFtrReader : IFlightAppObjectFtrReader
{
    private readonly IDictionary<string, IFlightAppObjectFtrReader> factories;
    public FlightAppFtrReader()
    {
        factories = new Dictionary<string, IFlightAppObjectFtrReader>()
         {
             {"C", new CrewFtrReader() },
             {"P", new PassangerFtrReader() },
             {"CA", new CargoFtrReader() },
             {"PP", new PassangerPlaneFtrReader() },
             {"CP", new CargoPlaneFtrReader() },
             {"AI", new AirportFtrReader() },
             {"FL", new FlightFtrReader() }
         };
    }

    public void AddToFlightAppDataUpdate(string[] data, IFlightAppDataUpdate flightAppDataUpdate)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (data.Length < 1)
        {
            throw new ArgumentException(nameof(data));
        }

        if (!factories.TryGetValue(data[0], out var factory))
        {
            throw new ArgumentOutOfRangeException(nameof(data));
        }

        factory.AddToFlightAppDataUpdate(data, flightAppDataUpdate);
    }
}

internal class CrewFtrReader : IFlightAppObjectFtrReader
{
    public void AddToFlightAppDataUpdate(string[] data, IFlightAppDataUpdate flightAppDataUpdate)
    {
        if (data.Length != 8)
        {
            throw new ArgumentException(nameof(data));
        }

        flightAppDataUpdate.Add(new Crew(
            ulong.Parse(data[1], CultureInfo.InvariantCulture),
            data[2],
            ulong.Parse(data[3], CultureInfo.InvariantCulture),
            data[4],
            data[5],
            ushort.Parse(data[6], CultureInfo.InvariantCulture),
            data[7]));
    }
}

internal class PassangerFtrReader : IFlightAppObjectFtrReader
{
    public void AddToFlightAppDataUpdate(string[] data, IFlightAppDataUpdate flightAppDataUpdate)
    {
        if (data.Length != 8)
        {
            throw new ArgumentException(nameof(data));
        }

        flightAppDataUpdate.Add(new Passanger(
            ulong.Parse(data[1], CultureInfo.InvariantCulture),
            data[2],
            ulong.Parse(data[3], CultureInfo.InvariantCulture),
            data[4],
            data[5],
            data[6],
            ulong.Parse(data[7], CultureInfo.InvariantCulture)));
    }
}

internal class CargoFtrReader : IFlightAppObjectFtrReader
{
    public void AddToFlightAppDataUpdate(string[] data, IFlightAppDataUpdate flightAppDataUpdate)
    {
        if (data.Length != 5)
        {
            throw new ArgumentException(nameof(data));
        }

        flightAppDataUpdate.Add(new Cargo(
            ulong.Parse(data[1], CultureInfo.InvariantCulture),
            float.Parse(data[2], CultureInfo.InvariantCulture),
            data[3],
            data[4]));
    }
}

internal class CargoPlaneFtrReader : IFlightAppObjectFtrReader
{
    public void AddToFlightAppDataUpdate(string[] data, IFlightAppDataUpdate flightAppDataUpdate)
    {
        if (data.Length != 6)
        {
            throw new ArgumentException(nameof(data));
        }

        flightAppDataUpdate.Add(new CargoPlane(
            ulong.Parse(data[1], CultureInfo.InvariantCulture),
            data[2],
            data[3],
            data[4],
            float.Parse(data[5], CultureInfo.InvariantCulture)));
    }
}

internal class PassangerPlaneFtrReader : IFlightAppObjectFtrReader
{
    public void AddToFlightAppDataUpdate(string[] data, IFlightAppDataUpdate flightAppDataUpdate)
    {
        if (data.Length != 8)
        {
            throw new ArgumentException(nameof(data));
        }

        flightAppDataUpdate.Add(new PassangerPlane(
            ulong.Parse(data[1], CultureInfo.InvariantCulture),
            data[2],
            data[3],
            data[4],
            ushort.Parse(data[5], CultureInfo.InvariantCulture),
            ushort.Parse(data[6], CultureInfo.InvariantCulture),
            ushort.Parse(data[7], CultureInfo.InvariantCulture)));
    }
}

internal class AirportFtrReader : IFlightAppObjectFtrReader
{
    public void AddToFlightAppDataUpdate(string[] data, IFlightAppDataUpdate flightAppDataUpdate)
    {
        if (data.Length != 8)
        {
            throw new ArgumentException(nameof(data));
        }

        flightAppDataUpdate.Add(new Airport(
            ulong.Parse(data[1], CultureInfo.InvariantCulture),
            data[2],
            data[3],
            float.Parse(data[4], CultureInfo.InvariantCulture),
            float.Parse(data[5], CultureInfo.InvariantCulture),
            float.Parse(data[6], CultureInfo.InvariantCulture),
            data[7]));
    }
}

internal class FlightFtrReader : IFlightAppObjectFtrReader
{
    public void AddToFlightAppDataUpdate(string[] data, IFlightAppDataUpdate flightAppDataUpdate)
    {
        if (data.Length != 12)
        {
            throw new ArgumentException(nameof(data));
        }

        var crewTab = string.Concat(
            data[10]
            .Skip(1)
            .SkipLast(1))
            .Split(';')
            .Select(s => ulong.Parse(s, CultureInfo.InvariantCulture))
            .ToArray();

        var loadTab = string.Concat(
            data[11]
            .Skip(1)
            .SkipLast(1))
            .Split(';')
            .Select(s => ulong.Parse(s, CultureInfo.InvariantCulture))
            .ToArray();

        flightAppDataUpdate.Add(new Flight(
            ulong.Parse(data[1], CultureInfo.InvariantCulture),
            ulong.Parse(data[2], CultureInfo.InvariantCulture),
            ulong.Parse(data[3], CultureInfo.InvariantCulture),
            convertToTime(data[4]),
            convertToTime(data[5]),
            float.Parse(data[6], CultureInfo.InvariantCulture),
            float.Parse(data[7], CultureInfo.InvariantCulture),
            float.Parse(data[8], CultureInfo.InvariantCulture),
            ulong.Parse(data[9], CultureInfo.InvariantCulture),
            crewTab,
            loadTab));

        DateTime convertToTime(string dateInfo)
        {
            var now = DateTime.Now;
            var info = dateInfo.Split(":");
            int hour = int.Parse(info[0], CultureInfo.InvariantCulture);
            int minute = int.Parse(info[1], CultureInfo.InvariantCulture);

            return new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
        }
    }
}
