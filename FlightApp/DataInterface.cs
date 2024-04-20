using System.Text.Json.Serialization;

namespace FlightApp
{
    [JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
    [JsonDerivedType(typeof(ICrew))]
    [JsonDerivedType(typeof(IPassanger))]
    [JsonDerivedType(typeof(ICargo))]
    [JsonDerivedType(typeof(ICargoPlane))]
    [JsonDerivedType(typeof(IPassangerPlane))]
    [JsonDerivedType(typeof(IAirport))]
    [JsonDerivedType(typeof(IFlight))]
    internal interface IFlightAppObject
    {
        string ClassType { get; }
        ulong Id { get; }
    }

    internal interface IPerson: IFlightAppObject
    {
        ulong Age { get; }
        string Email { get; }
        string Name { get; }
        string Phone { get; }
    }

    internal interface ICrew: IPerson
    {
        ushort Practice { get; }
        string Role { get; }
    }

    internal interface IPassanger: IPerson
    {
        string Class { get; }
        ulong Miles { get; }
    }

    internal interface ICargo: IFlightAppObject
    {
        string Code { get; }
        string Description { get; }
        float Weight { get; }
    }

    internal interface IPlane: IFlightAppObject
    {
        string Country { get; }
        string Model { get; }
        string Serial { get; }
    }

    internal interface ICargoPlane: IPlane
    {
        float MaxLoad { get; }
    }

    internal interface IPassangerPlane: IPlane
    {
        ushort BuisnessClassSize { get; }
        ushort EconomyClassSize { get; }
        ushort FirstClassSize { get; }
    }

    internal interface IAirport: IFlightAppObject
    {
        float AMSL { get; }
        string Code { get; }
        string Country { get; }
        float Latitude { get; }
        float Longitude { get; }
        string Name { get; }
    }

    internal interface IFlight: IFlightAppObject
    {
        float? AMSL { get; }
        ulong[] CrewAsIDs { get; }
        DateTime LandingTime { get; }
        float? Latitude { get; }
        ulong[] LoadAsIDs { get; }
        float? Longitude { get; }
        ulong OriginAsID { get; }
        ulong PlaneID { get; }
        DateTime TakeoffTime { get; }
        ulong TargetAsID { get; }
    }
}
