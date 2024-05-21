namespace FlightApp.Query.Processing.Execution
{
    internal abstract class CreateRecordFlightAppObject : IFlightAppObject
    {
        public ulong Id { get; set; }
        public string ClassType { get; set; }
    }

    internal abstract class CreateRecordPerson : CreateRecordFlightAppObject, IPerson
    {
        public string Name { get; set; }
        public ulong Age { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    internal class CreateRecordCrew : CreateRecordPerson, ICrew
    {
        public ushort Practice { get; set; }
        public string Role { get; set; }
    }

    internal class CreateRecordPassenger : CreateRecordPerson, IPassenger
    {
        public string Class { get; set; }
        public ulong Miles { get; set; }
    }

    internal class CreateRecordCargo : CreateRecordFlightAppObject, ICargo
    {
        public float Weight { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }

    internal abstract class CreateRecordPlane : CreateRecordFlightAppObject, IPlane
    {
        public string Serial { get; set; }
        public string Country { get; set; }
        public string Model { get; set; }
    }

    internal class CreateRecordCargoPlane : CreateRecordPlane, ICargoPlane
    {
        public float MaxLoad { get; set; }
    }

    internal class CreateRecordPassengerPlane : CreateRecordPlane, IPassengerPlane
    {
        public ushort FirstClassSize { get; set; }
        public ushort BusinessClassSize { get; set; }
        public ushort EconomyClassSize { get; set; }
    }

    internal class CreateRecordAirport : CreateRecordFlightAppObject, IAirport
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public float AMSL { get; set; }
        public string Country { get; set; }
    }

    internal class CreateRecordFlight : CreateRecordFlightAppObject, IFlight
    {
        public ulong OriginAsID { get; set; }
        public ulong TargetAsID { get; set; }
        public DateTime TakeoffTime { get; set; }
        public DateTime LandingTime { get; set; }
        public float? Longitude { get; set; }
        public float? Latitude { get; set; }
        public float? AMSL { get; set; }
        public ulong PlaneID { get; set; }
        public ulong[] CrewAsIDs { get; set; }
        public ulong[] LoadAsIDs { get; set; }
        public DateTime? LastPositionTime => null;
    }
}
