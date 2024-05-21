
using FlightApp.Logger;
using FlightApp.Query;
using FlightApp.Query.Processing.Execution;
using System.Globalization;

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

    internal interface IPassengerUpdateDecorator : IPersonUpdateDecorator, IPassenger
    {
    }

    internal interface ICargoPlaneUpdateDecorator : IFlightAppObjectUpdateDecorator, ICargoPlane
    {
    }

    internal interface IPassengerPlaneUpdateDecorator : IFlightAppObjectUpdateDecorator, IPassengerPlane
    {
    }

    internal interface IAirportUpdateDecorator : IFlightAppObjectUpdateDecorator, IAirport
    {
        public void ProcessQueryUpdate(IDictionary<string, string> updateData);
    }

    internal interface IFlightUpdateDecorator : IFlightAppObjectUpdateDecorator, IFlight
    {
        void UpdateData(PositionUpdateData updateData, ILogger logger);
    }

    internal abstract class FlightAppObjectUpdateDecorator<TDecoratedType> : IFlightAppObjectUpdateDecorator
        where TDecoratedType : IFlightAppObject
    {
        private ulong? idUpdated;

        protected FlightAppObjectUpdateDecorator(TDecoratedType decoratedObject)
        {
            Decorated = decoratedObject;
        }

        protected TDecoratedType Decorated { get; }

        public string ClassType => Decorated.ClassType;

        public ulong Id 
        {
            get => idUpdated ?? Decorated.Id;
            set => idUpdated = value;
        }

        public virtual void UpdateData(IDUpdateData updateData, ILogger logger)
        {
            if (updateData.ObjectID == Id)
            {
                logger.LogData($"Updating id: {Id} => {updateData.NewObjectID}");
                Id = updateData.NewObjectID;
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

    internal sealed class PassengerUpdateDecorator : PersonUpdateDecorator<IPassenger>, IPassengerUpdateDecorator
    {
        public PassengerUpdateDecorator(IPassenger passenger) : base(passenger)
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

    internal sealed class PassengerPlaneUpdateDecorator : PlaneUpdateDecorator<IPassengerPlane>, IPassengerPlaneUpdateDecorator
    {
        public PassengerPlaneUpdateDecorator(IPassengerPlane passengerPlane) : base(passengerPlane)
        {
        }

        public ushort BusinessClassSize => Decorated.BusinessClassSize;

        public ushort EconomyClassSize => Decorated.EconomyClassSize;

        public ushort FirstClassSize => Decorated.FirstClassSize;
    }

    internal sealed class AirportUpdateDecorator : FlightAppObjectUpdateDecorator<IAirport>, IAirportUpdateDecorator
    {
        private static readonly Lazy<IDictionary<string, Action<AirportUpdateDecorator, string>>> updateLibLazy = new(CreateUpdateDictionary);

        private float? AMSLUpdated;
        private string? CodeUpdated;
        private string? CountryUpdated;
        private float? LatitudeUpdated;
        private float? LongitudeUpdated;
        private string? NameUpdated;

        public AirportUpdateDecorator(IAirport airport) : base(airport)
        {
        }

        public void ProcessQueryUpdate(IDictionary<string, string> updateData)
        {
            foreach (var setValue in updateData)
            {
                if (updateLibLazy.Value.TryGetValue(setValue.Key, out var setFunc))
                {
                    try
                    {
                        setFunc(this, setValue.Value);
                    }
                    catch (Exception ex)
                    {
                        throw new QueryProcessingException($"setting {setValue.Key} value error", ex);
                    }
                }
                else
                {
                    throw new QueryProcessingException($"setter for {setValue.Key} not found");
                }
            }

        }

        public float AMSL
        {
            get => AMSLUpdated ?? Decorated.AMSL;
            set => AMSLUpdated = value;
        }

        public string Code
        {
            get => CodeUpdated ?? Decorated.Code;
            set => CodeUpdated = value;
        }

        public string Country
        {
            get => CountryUpdated ?? Decorated.Country;
            set => CountryUpdated = value;
        }

        public float Latitude
        {
            get => LatitudeUpdated ?? Decorated.Latitude;
            set => LatitudeUpdated = value;
        }

        public float Longitude
        {
            get => LongitudeUpdated ?? Decorated.Longitude;
            set => LongitudeUpdated = value;
        }

        public string Name
        {
            get => NameUpdated ?? Decorated.Name;
            set => NameUpdated = value;
        }

        private static Dictionary<string, Action<AirportUpdateDecorator, string>> CreateUpdateDictionary() =>
            new()
            {
                {QuerySyntax.Airport.IdField, (airport, value) => airport.Id = ulong.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.Airport.AmslField, (airport, value) => airport.AMSL = float.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.Airport.CodeField, (airport, value) => airport.Code = value },
                {QuerySyntax.Airport.CountryCodeField, (airport, value) => airport.Country = value },
                {QuerySyntax.Airport.NameField, (airport, value) => airport.Name = value },
                {$"{QuerySyntax.Airport.WorldPositionField}.{QuerySyntax.WorldPosition.LongitudeField}", (airport, value) => airport.Longitude = float.Parse(value, CultureInfo.CurrentUICulture) },
                {$"{QuerySyntax.Airport.WorldPositionField}.{QuerySyntax.WorldPosition.LatitudeField}", (airport, value) => airport.Longitude = float.Parse(value, CultureInfo.CurrentUICulture) },
            };

    }

    internal sealed class FlightUpdateDecorator : FlightAppObjectUpdateDecorator<IFlight>, IFlightUpdateDecorator
    {
        private ulong? updatedOriginAsID;
        private ulong? updatedTargetAsID;
        private ulong? updatedPlaneID;
        private ulong[]? updatedCrewAsIDs;
        private ulong[]? updatedLoadAsIDs;
        private float? updatedLatitude;
        private float? updatedLongitude;
        private float? updatedAMSL;

        public FlightUpdateDecorator(IFlight flight) : base(flight)
        {
        }

        public float? AMSL => updatedAMSL ?? Decorated.AMSL;

        public ulong[] CrewAsIDs => updatedCrewAsIDs ?? Decorated.CrewAsIDs;

        public DateTime LandingTime => Decorated.LandingTime;

        public float? Latitude => updatedLatitude ?? Decorated.Latitude;

        public ulong[] LoadAsIDs => updatedLoadAsIDs ?? Decorated.LoadAsIDs;

        public float? Longitude => updatedLongitude ?? Decorated.Longitude;

        public ulong OriginAsID => updatedOriginAsID ?? Decorated.OriginAsID;

        public ulong PlaneID => updatedPlaneID ?? Decorated.PlaneID;

        public DateTime TakeoffTime => Decorated.TakeoffTime;

        public ulong TargetAsID => updatedTargetAsID ?? Decorated.TargetAsID;

        public DateTime? LastPositionTime { get; private set; }

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

        public void UpdateData(PositionUpdateData updateData, ILogger logger)
        {
            if (updateData.ObjectID == Id)
            {
                logger.LogData($"Updating flight position info: id={Id} " +
                    $"| amsl {AMSL} => {updateData.AMSL} " +
                    $"| latitude {Latitude} => {updateData.Latitude}" +
                    $"| longitude {Longitude} => {updateData.Longitude}");

                updatedAMSL = updateData.AMSL;
                updatedLatitude = updateData.Latitude;
                updatedLongitude = updateData.Longitude;
            }
        }
    }

}
