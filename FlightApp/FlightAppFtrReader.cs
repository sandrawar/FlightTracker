﻿using System.Data;
using System.Globalization;

namespace FlightApp;

internal interface IFlightAppFtrReader
{
    FlightAppObject Create(string[] details);
}

internal class FlightAppFtrReader : IFlightAppFtrReader 
{
    private readonly IDictionary<string, IFlightAppFtrReader> factories;
    public FlightAppFtrReader()
    {
         factories = new Dictionary<string, IFlightAppFtrReader>()
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

    public FlightAppObject Create(string[] details)
    {
        if (details == null)
        {
            throw new ArgumentNullException(nameof(details));
        }

        if (details.Length < 1)
        {
            throw new ArgumentException(nameof(details));
        }

        if (!factories.TryGetValue(details[0], out var factory))
        {
            throw new ArgumentOutOfRangeException(nameof(details));
        }

        return factory.Create(details);
    }
}

internal class CrewFtrReader : IFlightAppFtrReader
{
    public FlightAppObject Create(string[] details)
    {
        if (details.Length != 8)
        {
            throw new ArgumentException(nameof(details));
        }

        return new Crew(
            ulong.Parse(details[1], CultureInfo.InvariantCulture), 
            details[2], 
            ulong.Parse(details[3], CultureInfo.InvariantCulture), 
            details[4], 
            details[5], 
            ushort.Parse(details[6], CultureInfo.InvariantCulture), 
            details[7]);
    }
}

internal class PassangerFtrReader : IFlightAppFtrReader
{
    public FlightAppObject Create(string[] details)
    {
        if (details.Length != 8)
        {
            throw new ArgumentException(nameof(details));
        }

        return new Passanger(
            ulong.Parse(details[1], CultureInfo.InvariantCulture), 
            details[2], 
            ulong.Parse(details[3], CultureInfo.InvariantCulture), 
            details[4], 
            details[5], 
            details[6], 
            ulong.Parse(details[7], CultureInfo.InvariantCulture));
    }
}

internal class CargoFtrReader : IFlightAppFtrReader
{
    public FlightAppObject Create(string[] details)
    {
        if (details.Length != 5)
        {
            throw new ArgumentException(nameof(details));
        }

        return new Cargo(
            ulong.Parse(details[1], CultureInfo.InvariantCulture), 
            float.Parse(details[2], CultureInfo.InvariantCulture), 
            details[3], 
            details[4]);
    }
}

internal class CargoPlaneFtrReader : IFlightAppFtrReader
{
    public FlightAppObject Create(string[] details)
    {
        if (details.Length != 6)
        {
            throw new ArgumentException(nameof(details));
        }

        return new CargoPlane(
            ulong.Parse(details[1], CultureInfo.InvariantCulture), 
            details[2], 
            details[3], 
            details[4], 
            float.Parse(details[5], CultureInfo.InvariantCulture));
    }
}

internal class PassangerPlaneFtrReader : IFlightAppFtrReader
{
    public FlightAppObject Create(string[] details)
    {
        if (details.Length != 8)
        {
            throw new ArgumentException(nameof(details));
        }

        return new PassangerPlane(
            ulong.Parse(details[1], CultureInfo.InvariantCulture), 
            details[2], 
            details[3], 
            details[4], 
            ushort.Parse(details[5], CultureInfo.InvariantCulture), 
            ushort.Parse(details[6], CultureInfo.InvariantCulture), 
            ushort.Parse(details[7], CultureInfo.InvariantCulture));
    }
}

internal class AirportFtrReader : IFlightAppFtrReader
{
    public FlightAppObject Create(string[] details)
    {
        if (details.Length != 8)
        {
            throw new ArgumentException(nameof(details));
        }

        return new Airport(
            ulong.Parse(details[1], CultureInfo.InvariantCulture), 
            details[2], 
            details[3], 
            float.Parse(details[4], CultureInfo.InvariantCulture), 
            float.Parse(details[5], CultureInfo.InvariantCulture), 
            float.Parse(details[6], CultureInfo.InvariantCulture), 
            details[7]);
    }
}

internal class FlightFtrReader : IFlightAppFtrReader
{
    public FlightAppObject Create(string[] details)
    {
        if (details.Length != 12)
        {
            throw new ArgumentException(nameof(details));
        }

        var crewTab = string.Concat(
            details[10]
            .Skip(1)
            .SkipLast(1))
            .Split(';')
            .Select(s => ulong.Parse(s, CultureInfo.InvariantCulture))
            .ToArray();

        var loadTab = string.Concat(
            details[11]
            .Skip(1)
            .SkipLast(1))
            .Split(';')
            .Select(s => ulong.Parse(s, CultureInfo.InvariantCulture))
            .ToArray();

        return new Flight(
            ulong.Parse(details[1], CultureInfo.InvariantCulture),
            ulong.Parse(details[2], CultureInfo.InvariantCulture),
            ulong.Parse(details[3], CultureInfo.InvariantCulture),
            convertToTime(details[4]),
            convertToTime(details[5]),
            float.Parse(details[6], CultureInfo.InvariantCulture),
            float.Parse(details[7], CultureInfo.InvariantCulture),
            float.Parse(details[8], CultureInfo.InvariantCulture),
            ulong.Parse(details[9], CultureInfo.InvariantCulture),
            crewTab,
            loadTab);

        DateTime convertToTime(string data)
        {
            var now = DateTime.Now;
            var info = data.Split(":");
            int hour = int.Parse(info[0], CultureInfo.InvariantCulture);
            int minute = int.Parse(info[1], CultureInfo.InvariantCulture);

            return new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
        }
    }
}
