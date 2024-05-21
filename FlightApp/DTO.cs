namespace FlightApp;

internal abstract class FlightAppObject : IFlightAppObject
{
    protected FlightAppObject(string classType, ulong id)
    {
        ClassType = classType ?? throw new ArgumentNullException(nameof(classType));
        Id = id;
    }

    public ulong Id { get; }

    public string ClassType { get; }
}

internal abstract class Person : FlightAppObject, IPerson
{
    protected Person(string classType, ulong id, string name, ulong age, string phone, string email)
        : base(classType, id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Age = age;
        Phone = phone ?? throw new ArgumentNullException(nameof(phone));
        Email = email ?? throw new ArgumentNullException(nameof(email));
    }

    public string Name { get; }

    public ulong Age { get; }

    public string Phone { get; }

    public string Email { get; }
}

internal class Crew : Person, ICrew
{
    public Crew(ulong id, string name, ulong age, string phone, string email, ushort practice, string role)
        : base("C", id, name, age, phone, email)
    {
        Practice = practice;
        Role = role ?? throw new ArgumentNullException(nameof(role));
    }

    public ushort Practice { get; }

    public string Role { get; }
}

internal class Passenger : Person, IPassenger
{
    public Passenger(ulong id, string name, ulong age, string phone, string email, string @class, ulong miles)
        : base("P", id, name, age, phone, email)

    {
        Class = @class ?? throw new ArgumentNullException(nameof(@class));
        Miles = miles;
    }

    public string Class { get; }

    public ulong Miles { get; }
}

internal class Cargo : FlightAppObject, ICargo
{
    public Cargo(ulong id, float weight, string code, string description)
        : base("CA", id)
    {
        Weight = weight;
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Description = description ?? throw new ArgumentNullException(nameof(description));
    }

    public float Weight { get; }

    public string Code { get; }

    public string Description { get; }
}

internal abstract class Plane : FlightAppObject, IPlane
{
    protected Plane(string classType, ulong id, string serial, string country, string model)
        : base(classType, id)
    {
        Serial = serial ?? throw new ArgumentNullException(nameof(serial));
        Country = country ?? throw new ArgumentNullException(nameof(country));
        Model = model ?? throw new ArgumentNullException(nameof(model));
    }

    public string Serial { get; }

    public string Country { get; }

    public string Model { get; }
}

internal class CargoPlane : Plane, ICargoPlane
{
    public CargoPlane(ulong id, string serial, string country, string model, float maxLoad)
        : base("CP", id, serial, country, model)
    {
        MaxLoad = maxLoad;
    }

    public float MaxLoad { get; }
}

internal class PassengerPlane : Plane, IPassengerPlane
{
    public PassengerPlane(ulong id, string serial, string country, string model, ushort firstClassSize, ushort buisnessClassSize, ushort economyClassSize)
        : base("PP", id, serial, country, model)
    {
        FirstClassSize = firstClassSize;
        BusinessClassSize = buisnessClassSize;
        EconomyClassSize = economyClassSize;
    }

    public ushort FirstClassSize { get; }

    public ushort BusinessClassSize { get; }

    public ushort EconomyClassSize { get; }
}

internal class Airport : FlightAppObject, IAirport
{
    public Airport(ulong id, string name, string code, float longitude, float latitude, float aMSL, string country)
        : base("AI", id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Longitude = longitude;
        Latitude = latitude;
        AMSL = aMSL;
        Country = country;
    }

    public string Name { get; }

    public string Code { get; }

    public float Longitude { get; }

    public float Latitude { get; }

    public float AMSL { get; }

    public string Country { get; }
}

internal class Flight : FlightAppObject, IFlight
{
    public Flight(
        ulong id,
        ulong originAsID,
        ulong targetAsID,
        DateTime takeoffTime,
        DateTime landingTime,
        float? longitude,
        float? latitude,
        float? aMSL,
        ulong planeID,
        ulong[] crewAsIDs,
        ulong[] loadAsIDs)
        : base("FL", id)
    {
        OriginAsID = originAsID;
        TargetAsID = targetAsID;
        TakeoffTime = takeoffTime;
        LandingTime = landingTime;
        Longitude = longitude;
        Latitude = latitude;
        AMSL = aMSL;
        PlaneID = planeID;
        CrewAsIDs = crewAsIDs ?? throw new ArgumentNullException(nameof(crewAsIDs));
        LoadAsIDs = loadAsIDs ?? throw new ArgumentNullException(nameof(loadAsIDs));
    }

    public ulong OriginAsID { get; }

    public ulong TargetAsID { get; }

    public DateTime TakeoffTime { get; }

    public DateTime LandingTime { get; }

    public float? Longitude { get; }

    public float? Latitude { get; }

    public float? AMSL { get; }

    public ulong PlaneID { get; }

    public ulong[] CrewAsIDs { get; }

    public ulong[] LoadAsIDs { get; }

    public DateTime? LastPositionTime => null;
}
