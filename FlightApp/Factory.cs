using System.Data;
using System.Globalization;

namespace FlightApp;

internal interface IFlightAppObjectFactory
{
    FlightAppObject Create(string[] data);
}

internal class FlightAppDataFactory : IFlightAppObjectFactory 
{
    private readonly IDictionary<string, IFlightAppObjectFactory> factories;
    public FlightAppDataFactory()
    {
         factories = new Dictionary<string, IFlightAppObjectFactory>()
         {
             {"C", new CrewFactory() },
             {"P", new PassangerFactory() },
             {"CA", new CargoFactory() },
             {"PP", new PassangerPlaneFactory() },
             {"CP", new CargoPlaneFactory() },
             {"AI", new AirportFactory() },
             {"FL", new FlightFactory() }
         };   
    }

    public FlightAppObject Create(string[] data)
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

        return factory.Create(data);
    }
}

internal class CrewFactory : IFlightAppObjectFactory
{
    public FlightAppObject Create(string[] data)
    {
        if (data.Length != 8)
        {
            throw new ArgumentException(nameof(data));
        }

        return new Crew(data[0], ulong.Parse(data[1], CultureInfo.InvariantCulture), data[2], ulong.Parse(data[3], CultureInfo.InvariantCulture), data[4], data[5], ushort.Parse(data[6], CultureInfo.InvariantCulture), data[7]);
    }
}

internal class PassangerFactory : IFlightAppObjectFactory
{
    public FlightAppObject Create(string[] data)
    {
        if (data.Length != 8)
        {
            throw new ArgumentException(nameof(data));
        }

        return new Passanger(data[0], ulong.Parse(data[1], CultureInfo.InvariantCulture), data[2], ulong.Parse(data[3], CultureInfo.InvariantCulture), data[4], data[5], data[6], ulong.Parse(data[7], CultureInfo.InvariantCulture));
    }
}

internal class CargoFactory : IFlightAppObjectFactory
{
    public FlightAppObject Create(string[] data)
    {
        if (data.Length != 5)
        {
            throw new ArgumentException(nameof(data));
        }

        return new Cargo(data[0], ulong.Parse(data[1], CultureInfo.InvariantCulture), float.Parse(data[2], CultureInfo.InvariantCulture), data[3], data[4]);
    }
}

internal class CargoPlaneFactory : IFlightAppObjectFactory
{
    public FlightAppObject Create(string[] data)
    {
        if (data.Length != 6)
        {
            throw new ArgumentException(nameof(data));
        }

        return new CargoPlane(data[0], ulong.Parse(data[1], CultureInfo.InvariantCulture), data[2], data[3], data[4], float.Parse(data[5], CultureInfo.InvariantCulture));
    }
}

internal class PassangerPlaneFactory : IFlightAppObjectFactory
{
    public FlightAppObject Create(string[] data)
    {
        if (data.Length != 8)
        {
            throw new ArgumentException(nameof(data));
        }

        return new PassangerPlane(data[0], ulong.Parse(data[1], CultureInfo.InvariantCulture), data[2], data[3], data[4], ushort.Parse(data[5], CultureInfo.InvariantCulture), ushort.Parse(data[6], CultureInfo.InvariantCulture), ushort.Parse(data[7], CultureInfo.InvariantCulture));
    }
}

internal class AirportFactory : IFlightAppObjectFactory
{
    public FlightAppObject Create(string[] data)
    {
        if (data.Length != 8)
        {
            throw new ArgumentException(nameof(data));
        }

        return new Airport(data[0], ulong.Parse(data[1], CultureInfo.InvariantCulture), data[2], data[3], float.Parse(data[4], CultureInfo.InvariantCulture), float.Parse(data[5], CultureInfo.InvariantCulture), float.Parse(data[6], CultureInfo.InvariantCulture), data[7]);
    }
}

internal class FlightFactory : IFlightAppObjectFactory
{
    public FlightAppObject Create(string[] data)
    {
        if (data.Length != 12)
        {
            throw new ArgumentException(nameof(data));
        }

        var crewTab = string.Concat(data[10].Skip(1).SkipLast(1)).Split(';').Select(s => ulong.Parse(s, CultureInfo.InvariantCulture)).ToArray();
        var loadTab = string.Concat(data[11].Skip(1).SkipLast(1)).Split(';').Select(s => ulong.Parse(s, CultureInfo.InvariantCulture)).ToArray();

        return new Flight(data[0], ulong.Parse(data[1], CultureInfo.InvariantCulture), ulong.Parse(data[2], CultureInfo.InvariantCulture), ulong.Parse(data[3], CultureInfo.InvariantCulture), data[4], data[5], float.Parse(data[6], CultureInfo.InvariantCulture), float.Parse(data[7], CultureInfo.InvariantCulture), float.Parse(data[8], CultureInfo.InvariantCulture), ulong.Parse(data[9], CultureInfo.InvariantCulture), crewTab, loadTab);
    }
}
