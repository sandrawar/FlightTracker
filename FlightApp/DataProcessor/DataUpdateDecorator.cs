
namespace FlightApp.DataProcessor
{
    internal interface IFlightAppObjectUpdateDecorator: IFlightAppObject
    {
    }

    internal interface ICargoUpdateDecorator : IFlightAppObjectUpdateDecorator, ICargo
    {
    }

    internal interface ICrewUpdateDecorator : IFlightAppObjectUpdateDecorator, ICrew
    {
    }

    internal interface IPassangerUpdateDecorator : IFlightAppObjectUpdateDecorator, IPassanger
    {
    }

    internal interface ICargoPlaneUpdateDecorator : IFlightAppObjectUpdateDecorator, ICargoPlane
    {
    }

    internal interface IPassangerPlaneUpdateDecorator : IFlightAppObjectUpdateDecorator, IPassangerPlane
    {
    }

    internal interface IAirportUpdateDecorator : IFlightAppObjectUpdateDecorator, IAirport
    {
    }

    internal interface IFlightUpdateDecorator : IFlightAppObjectUpdateDecorator, IFlight
    {
    }

    internal abstract class FlightAppObjectUpdateDecorator<TDecoratedType>: IFlightAppObjectUpdateDecorator
        where TDecoratedType: IFlightAppObject
    {
        private ulong? actualId;

        protected FlightAppObjectUpdateDecorator(TDecoratedType decoratedObject)
        {
            Decorated = decoratedObject;
            actualId = null;
        }

        protected TDecoratedType Decorated { get; init; }

        public string ClassType => Decorated.ClassType;

        public ulong Id {get => actualId ?? Decorated.Id; }
    }

    internal sealed class CargoUpdateDecorator : FlightAppObjectUpdateDecorator<ICargo>, ICargoUpdateDecorator
    {
        public CargoUpdateDecorator(ICargo cargo) : base(cargo)
        {
        }

        public string Code => Decorated.Code;

        public string Description => Decorated.Description;

        public float Weight => Decorated.Weight;
    }

    internal abstract class PersonUpdateDecorator<TDecoratedType> : FlightAppObjectUpdateDecorator<TDecoratedType>, IPerson
        where TDecoratedType: IPerson
    {
        public PersonUpdateDecorator(TDecoratedType person) : base(person)
        {
        }

        public ulong Age => Decorated.Age;

        public string Email => Decorated.Email;

        public string Name => Decorated.Name;

        public string Phone => Decorated.Phone;
    }

    internal sealed class CrewUpdateDecorator : PersonUpdateDecorator<ICrew>, ICrewUpdateDecorator
    {
        public CrewUpdateDecorator(ICrew crew) : base(crew)
        {
        }

        public ushort Practice => Decorated.Practice;

        public string Role => Decorated.Role;
    }

    internal sealed class PassangerUpdateDecorator : PersonUpdateDecorator<IPassanger>, IPassangerUpdateDecorator
    {
        public PassangerUpdateDecorator(IPassanger passenger) : base(passenger)
        {
        }

        public string Class => Decorated.Class;

        public ulong Miles => Decorated.Miles;
    }

    internal abstract class PlaneUpdateDecorator<TDecoratedType> : FlightAppObjectUpdateDecorator<TDecoratedType>, IPlane
        where TDecoratedType : IPlane
    {
        public PlaneUpdateDecorator(TDecoratedType plane) : base(plane)
        {
        }

        public string Country => Decorated.Country;

        public string Model => Decorated.Model;

        public string Serial => Decorated.Serial;
    }

    internal sealed class CargoPlaneUpdateDecorator : PlaneUpdateDecorator<ICargoPlane>, ICargoPlaneUpdateDecorator
    {
        public CargoPlaneUpdateDecorator(ICargoPlane cargoPlane) : base(cargoPlane)
        {
        }

        public float MaxLoad => Decorated.MaxLoad;
    }

    internal sealed class PassangerPlaneUpdateDecorator : PlaneUpdateDecorator<IPassangerPlane>, IPassangerPlaneUpdateDecorator
    {
        public PassangerPlaneUpdateDecorator(IPassangerPlane passangerPlane) : base(passangerPlane)
        {
        }

        public ushort BuisnessClassSize => Decorated.BuisnessClassSize;

        public ushort EconomyClassSize => Decorated.EconomyClassSize;

        public ushort FirstClassSize => Decorated.FirstClassSize;
    }

    internal sealed class AirportUpdateDecorator : FlightAppObjectUpdateDecorator<IAirport>, IAirportUpdateDecorator
    {
        public AirportUpdateDecorator(IAirport airport) : base(airport)
        {
        }

        public float AMSL => Decorated.AMSL;

        public string Code => Decorated.Code;

        public string Country => Decorated.Country;

        public float Latitude => Decorated.Latitude;

        public float Longitude => Decorated.Longitude;

        public string Name => Decorated.Name;
    }

    internal sealed class FlightUpdateDecorator : FlightAppObjectUpdateDecorator<IFlight>, IFlightUpdateDecorator
    {
        public FlightUpdateDecorator(IFlight flight) : base(flight)
        {
        }

        public float? AMSL => Decorated.AMSL;

        public ulong[] CrewAsIDs => Decorated.CrewAsIDs;

        public DateTime LandingTime => Decorated.LandingTime;

        public float? Latitude => Decorated.Latitude;

        public ulong[] LoadAsIDs => Decorated.LoadAsIDs;

        public float? Longitude => Decorated.Longitude;

        public ulong OriginAsID => Decorated.OriginAsID;

        public ulong PlaneID => Decorated.PlaneID;

        public DateTime TakeoffTime => Decorated.TakeoffTime;

        public ulong TargetAsID => Decorated.TargetAsID;
    }

}
