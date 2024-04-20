
using FlightApp.Logger;
using NetworkSourceSimulator;

namespace FlightApp.DataProcessor
{
    internal interface IFlightAppObjectUpdateDecorator : IFlightAppObject
    {
        void UpdateData(IDUpdateData updateData, ILogger logger);
    }

    internal interface ICargoUpdateDecorator : IFlightAppObjectUpdateDecorator, ICargo
    {
    }

    internal interface IPersonUpdateDecorator : IFlightAppObjectUpdateDecorator, IPerson
    {
        void UpdateData(ContactInfoUpdateData updateData, ILogger logger);
    }

    internal interface ICrewUpdateDecorator : IPersonUpdateDecorator, ICrew
    {
    }

    internal interface IPassangerUpdateDecorator : IPersonUpdateDecorator, IPassanger
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

    internal abstract class FlightAppObjectUpdateDecorator<TDecoratedType> : IFlightAppObjectUpdateDecorator
        where TDecoratedType : IFlightAppObject
    {
        private ulong? idUpdated;

        protected FlightAppObjectUpdateDecorator(TDecoratedType decoratedObject)
        {
            Decorated = decoratedObject;
        }

        protected TDecoratedType Decorated { get; init; }

        public string ClassType => Decorated.ClassType;

        public ulong Id { get => idUpdated ?? Decorated.Id; }

        public virtual void UpdateData(IDUpdateData updateData, ILogger logger)
        {
            if (updateData.ObjectID == Id)
            {
                logger.LogData($"Updating id: {Id} => {updateData.NewObjectID}");

                idUpdated = updateData.NewObjectID;
            }
        }
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

    internal abstract class PersonUpdateDecorator<TDecoratedType> : FlightAppObjectUpdateDecorator<TDecoratedType>, IPersonUpdateDecorator
        where TDecoratedType : IPerson
    {
        private string? emailUpdated;
        private string? phoneUpdated;
        public PersonUpdateDecorator(TDecoratedType person) : base(person)
        {
        }

        public void UpdateData(ContactInfoUpdateData updateData, ILogger logger)
        {
            if (updateData.ObjectID == Id)
            {
                logger.LogData($"Updating contact info: id={Id} " +
                    $"| email {Email} => {updateData.EmailAddress} " +
                    $"| phone {Phone} => {updateData.PhoneNumber}");

                emailUpdated = updateData.EmailAddress;
                phoneUpdated = updateData.PhoneNumber;

            }
        }

        public ulong Age => Decorated.Age;

        public string Email => emailUpdated ?? Decorated.Email;

        public string Name => Decorated.Name;

        public string Phone => phoneUpdated ?? Decorated.Phone;
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
        private ulong? updatedOriginAsID;
        private ulong? updatedTargetAsID;
        private ulong? updatedPlaneID;
        private ulong[]? updatedCrewAsIDs;
        private ulong[]? updatedLoadAsIDs;

        public FlightUpdateDecorator(IFlight flight) : base(flight)
        {
        }

        public float? AMSL => Decorated.AMSL;

        public ulong[] CrewAsIDs => updatedCrewAsIDs ?? Decorated.CrewAsIDs;

        public DateTime LandingTime => Decorated.LandingTime;

        public float? Latitude => Decorated.Latitude;

        public ulong[] LoadAsIDs => updatedLoadAsIDs ?? Decorated.LoadAsIDs;

        public float? Longitude => Decorated.Longitude;

        public ulong OriginAsID => updatedOriginAsID ?? Decorated.OriginAsID;

        public ulong PlaneID => updatedPlaneID ?? Decorated.PlaneID;

        public DateTime TakeoffTime => Decorated.TakeoffTime;

        public ulong TargetAsID => updatedTargetAsID ?? Decorated.TargetAsID;

        public override void UpdateData(IDUpdateData updateData, ILogger logger)
        {
            base.UpdateData(updateData, logger);

            if (updateData.ObjectID == OriginAsID)
            {
                logger.LogData($"Updating flight {Id} {nameof(OriginAsID)}: {OriginAsID} => {updateData.NewObjectID}");
                updatedOriginAsID = updateData.NewObjectID;
            }
            if (updateData.ObjectID == TargetAsID)
            {
                logger.LogData($"Updating flight {Id} {nameof(TargetAsID)}: {TargetAsID} => {updateData.NewObjectID}");
                updatedTargetAsID = updateData.NewObjectID;
            }
            if (updateData.ObjectID == PlaneID)
            {
                logger.LogData($"Updating flight {Id} {nameof(PlaneID)}: {PlaneID} => {updateData.NewObjectID}");
                updatedPlaneID = updateData.NewObjectID;
            }
            if (CrewAsIDs.Contains(updateData.ObjectID))
            {
                logger.LogData($"Updating flight {Id} {nameof(CrewAsIDs)}: {updateData.ObjectID} => {updateData.NewObjectID}");
                updatedCrewAsIDs = CrewAsIDs.Where(id => id != updateData.ObjectID).Concat([updateData.NewObjectID]).ToArray();
            }
            if (LoadAsIDs.Contains(updateData.ObjectID))
            {
                logger.LogData($"Updating flight {Id} {nameof(LoadAsIDs)}: {updateData.ObjectID} => {updateData.NewObjectID}");
                updatedLoadAsIDs = LoadAsIDs.Where(id => id != updateData.ObjectID).Concat([updateData.NewObjectID]).ToArray();
            }
        }

    }

}
