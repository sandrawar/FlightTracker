using System.Text.Json.Serialization;

namespace FlightApp;

[JsonDerivedType(typeof(Crew))]
[JsonDerivedType(typeof(Passanger))]
[JsonDerivedType(typeof(Cargo))]
[JsonDerivedType(typeof(CargoPlane))]
[JsonDerivedType(typeof(PassangerPlane))]
[JsonDerivedType(typeof(Airport))]
[JsonDerivedType(typeof(Flight))]
internal abstract class FlightAppObject
{
    protected FlightAppObject(string classType, ulong id)
    {
        ClassType = classType ?? throw new ArgumentNullException(nameof(classType));
        Id = id;
    }

    public ulong Id { get; }

    public string ClassType { get; }
}

internal abstract class Person : FlightAppObject
{
    protected Person(string classType, ulong id, string name, ulong age, string phone, string email) : base(classType, id)
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

internal class Crew : Person
{
    public Crew(string classType, ulong id, string name, ulong age, string phone, string email, ushort practice, string role) : base(classType, id, name, age, phone, email)
    {
        Practice = practice;
        Role = role ?? throw new ArgumentNullException(nameof(role));
    }

    public ushort Practice { get; }

    public string Role { get; }
}

internal class Passanger : Person
{
    public Passanger(string classType, ulong id, string name, ulong age, string phone, string email, string @class, ulong miles) : base(classType, id, name, age, phone, email)

    {
        Class = @class ?? throw new ArgumentNullException(nameof(@class));
        Miles = miles;
    }

    public string Class { get; }

    public ulong Miles { get; }
}

internal class Cargo : FlightAppObject
{
    public Cargo(string classType, ulong id, float weight, string code, string description) : base(classType, id)
    {
        Weight = weight;
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Description = description ?? throw new ArgumentNullException(nameof(description));
    }

    public float Weight { get; }

    public string Code { get; }

    public string Description { get; }
}

internal abstract class Plane : FlightAppObject
{
    protected Plane(string classType, ulong id, string serial, string country, string model) : base(classType, id)
    {
        Serial = serial ?? throw new ArgumentNullException(nameof(serial));
        Country = country ?? throw new ArgumentNullException(nameof(country));
        Model = model ?? throw new ArgumentNullException(nameof(model));
    }

    public string Serial { get; }

    public string Country { get; }

    public string Model { get; }
}

internal class CargoPlane : Plane
{
    public CargoPlane(string classType, ulong id, string serial, string country, string model, float maxLoad) : base(classType, id, serial, country, model)
    {
        MaxLoad = maxLoad;
    }

    public float MaxLoad { get; }
}

internal class PassangerPlane : Plane
{
    public PassangerPlane(string classType, ulong id, string serial, string country, string model, ushort firstClassSize, ushort buisnessClassSize, ushort economyClassSize) : base(classType, id, serial, country, model)
    {
        FirstClassSize = firstClassSize;
        BuisnessClassSize = buisnessClassSize;
        EconomyClassSize = economyClassSize;
    }

    public ushort FirstClassSize { get; }

    public ushort BuisnessClassSize { get; }

    public ushort EconomyClassSize { get; }
}

internal class Airport : FlightAppObject
{
    public Airport(string classType, ulong id, string name, string code, float longitude, float latitude, float aMSL, string country) : base(classType, id)
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

internal class Flight : FlightAppObject
{
    public Flight(string classType, ulong id, ulong originAsID, ulong targetAsID, string takeoffTime, string landingTime, float longitude, float latitude, float aMSL, ulong planeID, ulong[] crewAsIDs, ulong[] loadAsIDs) : base(classType, id)
    {
        OriginAsID = originAsID;
        TargetAsID = targetAsID;
        TakeoffTime = takeoffTime ?? throw new ArgumentNullException(nameof(takeoffTime));
        LandingTime = landingTime ?? throw new ArgumentNullException(nameof(landingTime));
        Longitude = longitude;
        Latitude = latitude;
        AMSL = aMSL;
        PlaneID = planeID;
        CrewAsIDs = crewAsIDs ?? throw new ArgumentNullException(nameof(crewAsIDs));
        LoadAsIDs = loadAsIDs ?? throw new ArgumentNullException(nameof(loadAsIDs));
    }

    public ulong OriginAsID { get; }

    public ulong TargetAsID { get; }

    public string TakeoffTime { get; }

    public string LandingTime { get; }

    public float Longitude { get; }

    public float Latitude { get; }

    public float AMSL { get; }

    public ulong PlaneID { get; }

    public ulong[] CrewAsIDs { get; }

    public ulong[] LoadAsIDs { get; }
}
