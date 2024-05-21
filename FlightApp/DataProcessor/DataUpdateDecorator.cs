
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
        public void ProcessQueryUpdate(IDictionary<string, string> updateData);
    }

    internal interface IPersonUpdateDecorator : IFlightAppObjectUpdateDecorator, IPerson
    {
        void UpdateData(ContactInfoUpdateData updateData, ILogger logger);
    }

    internal interface ICrewUpdateDecorator : IPersonUpdateDecorator, ICrew
    {
        void ProcessQueryUpdate(IDictionary<string, string> updateData);
    }

    internal interface IPassengerUpdateDecorator : IPersonUpdateDecorator, IPassenger
    {
        void ProcessQueryUpdate(IDictionary<string, string> updateData);
    }

    internal interface ICargoPlaneUpdateDecorator : IFlightAppObjectUpdateDecorator, ICargoPlane
    {
        void ProcessQueryUpdate(IDictionary<string, string> updateData);
    }

    internal interface IPassengerPlaneUpdateDecorator : IFlightAppObjectUpdateDecorator, IPassengerPlane
    {
        void ProcessQueryUpdate(IDictionary<string, string> updateData);
    }

    internal interface IAirportUpdateDecorator : IFlightAppObjectUpdateDecorator, IAirport
    {
        void ProcessQueryUpdate(IDictionary<string, string> updateData);
    }

    internal interface IFlightUpdateDecorator : IFlightAppObjectUpdateDecorator, IFlight
    {
        void UpdateData(PositionUpdateData updateData, ILogger logger);
        void ProcessQueryUpdate(IDictionary<string, string> updateData);
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
        private static readonly Lazy<IDictionary<string, Action<CargoUpdateDecorator, string>>> updateLibLazy = new(CreateUpdateDictionary);

        private string? codeUpdated;
        private string? descriptionUpdated;
        private float? weightUpdated;

        public CargoUpdateDecorator(ICargo cargo) : base(cargo)
        {
        }

        public string Code
        {
            get => codeUpdated ?? Decorated.Code;
            set => codeUpdated = value;
        }

        public string Description
        {
            get => descriptionUpdated ?? Decorated.Description;
            set => descriptionUpdated = value;
        }

        public float Weight
        {
            get => weightUpdated ?? Decorated.Weight;
            set => weightUpdated = value;
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

        private static Dictionary<string, Action<CargoUpdateDecorator, string>> CreateUpdateDictionary() =>
            new()
            {
                {QuerySyntax.Cargo.IdField, (cargo, value) => cargo.Id = ulong.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.Cargo.CodeField, (cargo, value) => cargo.Code = value },
                {QuerySyntax.Cargo.DescriptionField, (cargo, value) => cargo.Description = value },
                {QuerySyntax.Cargo.WeightField, (cargo, value) => cargo.Weight = float.Parse(value, CultureInfo.CurrentUICulture) },
            };

    }

    internal abstract class PersonUpdateDecorator<TDecoratedType> : FlightAppObjectUpdateDecorator<TDecoratedType>, IPersonUpdateDecorator
        where TDecoratedType : IPerson
    {
        private ulong? ageUpdated;
        private string? emailUpdated;
        private string? nameUpdated;
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

        public ulong Age
        {
            get => ageUpdated ?? Decorated.Age;
            set => ageUpdated = value;
        }

        public string Email
        {
            get => emailUpdated ?? Decorated.Email;
            set => emailUpdated = value;
        }

        public string Name
        {
            get => nameUpdated ?? Decorated.Name;
            set => nameUpdated = value;
        }

        public string Phone
        {
            get => phoneUpdated ?? Decorated.Phone;
            set => phoneUpdated = value;
        }
    }

    internal sealed class CrewUpdateDecorator : PersonUpdateDecorator<ICrew>, ICrewUpdateDecorator
    {
        private static readonly Lazy<IDictionary<string, Action<CrewUpdateDecorator, string>>> updateLibLazy = new(CreateUpdateDictionary);

        private ushort? practiceUpdated;
        private string? roleUpdated;

        public CrewUpdateDecorator(ICrew crew) : base(crew)
        {
        }

        public ushort Practice
        {
            get => practiceUpdated ?? Decorated.Practice;
            set => practiceUpdated = value;
        }

        public string Role
        {
            get => roleUpdated ?? Decorated.Role;
            set => roleUpdated = value;
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

        private static Dictionary<string, Action<CrewUpdateDecorator, string>> CreateUpdateDictionary() =>
            new()
            {
                {QuerySyntax.Crew.IdField, (crew, value) => crew.Id = ulong.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.Crew.AgeField, (crew, value) => crew.Age = ulong.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.Crew.EmailField, (crew, value) => crew.Email = value },
                {QuerySyntax.Crew.NameField, (crew, value) => crew.Name = value },
                {QuerySyntax.Crew.PhoneField, (crew, value) => crew.Phone = value },
                {QuerySyntax.Crew.PractiseField, (crew, value) => crew.Practice = ushort.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.Crew.RoleField, (crew, value) => crew.Role = value },
            };

    }

    internal sealed class PassengerUpdateDecorator : PersonUpdateDecorator<IPassenger>, IPassengerUpdateDecorator
    {
        private static readonly Lazy<IDictionary<string, Action<PassengerUpdateDecorator, string>>> updateLibLazy = new(CreateUpdateDictionary);

        private string? classUpdated;
        private ulong? milesUpdated;

        public PassengerUpdateDecorator(IPassenger passenger) : base(passenger)
        {
        }

        public string Class
        {
            get => classUpdated ?? Decorated.Class;
            set => classUpdated = value;
        }

        public ulong Miles
        {
            get => milesUpdated ?? Decorated.Miles;
            set => milesUpdated = value;
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

        private static Dictionary<string, Action<PassengerUpdateDecorator, string>> CreateUpdateDictionary() =>
            new()
            {
                {QuerySyntax.Crew.IdField, (passenger, value) => passenger.Id = ulong.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.Crew.AgeField, (passenger, value) => passenger.Age = ulong.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.Passenger.ClassField, (passenger, value) => passenger.Class = value },
                {QuerySyntax.Crew.EmailField, (passenger, value) => passenger.Email = value },
                {QuerySyntax.Passenger.MilesField, (passenger, value) => passenger.Miles = ulong.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.Crew.NameField, (passenger, value) => passenger.Name = value },
                {QuerySyntax.Crew.PhoneField, (passenger, value) => passenger.Phone = value },
            };

    }

    internal abstract class PlaneUpdateDecorator<TDecoratedType> : FlightAppObjectUpdateDecorator<TDecoratedType>, IPlane
        where TDecoratedType : IPlane
    {
        private string? countryUpdated;
        private string? modelUpdated;
        private string? serialUpdated;

        public PlaneUpdateDecorator(TDecoratedType plane) : base(plane)
        {
        }

        public string Country
        {
            get => countryUpdated ?? Decorated.Country;
            set => countryUpdated = value;
        }

        public string Model
        {
            get => modelUpdated ?? Decorated.Model;
            set => modelUpdated = value;
        }

        public string Serial
        {
            get => serialUpdated ?? Decorated.Serial;
            set => serialUpdated = value;
        }
    }

    internal sealed class CargoPlaneUpdateDecorator : PlaneUpdateDecorator<ICargoPlane>, ICargoPlaneUpdateDecorator
    {
        private static readonly Lazy<IDictionary<string, Action<CargoPlaneUpdateDecorator, string>>> updateLibLazy = new(CreateUpdateDictionary);

        private float? MaxLoadUpdated;

        public CargoPlaneUpdateDecorator(ICargoPlane cargoPlane) : base(cargoPlane)
        {
        }

        public float MaxLoad
        {
            get => MaxLoadUpdated ?? Decorated.MaxLoad;
            set => MaxLoadUpdated = value;
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

        private static Dictionary<string, Action<CargoPlaneUpdateDecorator, string>> CreateUpdateDictionary() =>
            new()
            {
                {QuerySyntax.CargoPlane.IdField, (plane, value) => plane.Id = ulong.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.CargoPlane.CountryCodeField, (plane, value) => plane.Country = value },
                {QuerySyntax.CargoPlane.MaxLoadField, (plane, value) => plane.MaxLoad = float.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.CargoPlane.ModelField, (plane, value) => plane.Model = value },
                {QuerySyntax.CargoPlane.SerialField, (plane, value) => plane.Serial = value },
            };
    }

    internal sealed class PassengerPlaneUpdateDecorator : PlaneUpdateDecorator<IPassengerPlane>, IPassengerPlaneUpdateDecorator
    {
        private static readonly Lazy<IDictionary<string, Action<PassengerPlaneUpdateDecorator, string>>> updateLibLazy = new(CreateUpdateDictionary);

        private ushort? businessClassSizeUpdated;
        private ushort? economyClassSizeUpdated;
        private ushort? firstClassSizeUpdated;

        public PassengerPlaneUpdateDecorator(IPassengerPlane passengerPlane) : base(passengerPlane)
        {
        }

        public ushort BusinessClassSize
        {
            get => businessClassSizeUpdated ?? Decorated.BusinessClassSize;
            set => businessClassSizeUpdated = value; 
        }

        public ushort EconomyClassSize
        {
            get => economyClassSizeUpdated ?? Decorated.EconomyClassSize;
            set => economyClassSizeUpdated = value;
        }

        public ushort FirstClassSize
        {
            get => firstClassSizeUpdated ?? Decorated.FirstClassSize;
            set => firstClassSizeUpdated = value;
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

        private static Dictionary<string, Action<PassengerPlaneUpdateDecorator, string>> CreateUpdateDictionary() =>
            new()
            {
                {QuerySyntax.CargoPlane.IdField, (plane, value) => plane.Id = ulong.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.PassengerPlane.BusinessClassSizeField, (plane, value) => plane.BusinessClassSize = ushort.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.CargoPlane.CountryCodeField, (plane, value) => plane.Country = value },
                {QuerySyntax.PassengerPlane.EconomyClassSizeField, (plane, value) => plane.EconomyClassSize = ushort.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.PassengerPlane.FirstClassSizeField, (plane, value) => plane.FirstClassSize = ushort.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.CargoPlane.ModelField, (plane, value) => plane.Model = value },
                {QuerySyntax.CargoPlane.SerialField, (plane, value) => plane.Serial = value },
            };

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
        private static readonly Lazy<IDictionary<string, Action<FlightUpdateDecorator, string>>> updateLibLazy = new(CreateAddDictionary);

        private ulong? originAsIDUpdated;
        private ulong? targetAsIDUpdated;
        private ulong? planeIDUpdated;
        private ulong[]? crewAsIDsUpdated;
        private ulong[]? loadAsIDsUpdated;
        private float? latitudeUpdated;
        private float? longitudeUpdated;
        private float? updatedAMSL;
        private DateTime? landingTimeUpdated;
        private DateTime? takeoffTimeUpdated;

        public FlightUpdateDecorator(IFlight flight) : base(flight)
        {
        }

        public float? AMSL
        {
            get => updatedAMSL ?? Decorated.AMSL;
            set => updatedAMSL = value;
        }

        public ulong[] CrewAsIDs
        {
            get => crewAsIDsUpdated ?? Decorated.CrewAsIDs;
            set => crewAsIDsUpdated = value;
        }

        public DateTime LandingTime
        {
            get => landingTimeUpdated ?? Decorated.LandingTime;
            set => landingTimeUpdated = value;
        }

        public float? Latitude
        {
            get => latitudeUpdated ?? Decorated.Latitude;
            set => latitudeUpdated = value;
        }

        public ulong[] LoadAsIDs
        {
            get => loadAsIDsUpdated ?? Decorated.LoadAsIDs;
            set => loadAsIDsUpdated = value;
        }

        public float? Longitude
        {
            get => longitudeUpdated ?? Decorated.Longitude;
            set => longitudeUpdated = value;
        }

        public ulong OriginAsID
        {
            get => originAsIDUpdated ?? Decorated.OriginAsID;
            set => originAsIDUpdated = value;
        }

        public ulong PlaneID
        {
            get => planeIDUpdated ?? Decorated.PlaneID;
            set => planeIDUpdated = value;
        }

        public DateTime TakeoffTime
        {
            get => takeoffTimeUpdated ?? Decorated.TakeoffTime;
            set => takeoffTimeUpdated = value;
        }

        public ulong TargetAsID
        {
            get => targetAsIDUpdated ?? Decorated.TargetAsID;
            set => targetAsIDUpdated = value;
        }

        public DateTime? LastPositionTime { get; private set; }

        public override void UpdateData(IDUpdateData updateData, ILogger logger)
        {
            base.UpdateData(updateData, logger);

            if (updateData.ObjectID == OriginAsID)
            {
                logger.LogData($"Updating flight {Id} {nameof(OriginAsID)}: {OriginAsID} => {updateData.NewObjectID}");
                originAsIDUpdated = updateData.NewObjectID;
            }
            if (updateData.ObjectID == TargetAsID)
            {
                logger.LogData($"Updating flight {Id} {nameof(TargetAsID)}: {TargetAsID} => {updateData.NewObjectID}");
                targetAsIDUpdated = updateData.NewObjectID;
            }
            if (updateData.ObjectID == PlaneID)
            {
                logger.LogData($"Updating flight {Id} {nameof(PlaneID)}: {PlaneID} => {updateData.NewObjectID}");
                planeIDUpdated = updateData.NewObjectID;
            }
            if (CrewAsIDs.Contains(updateData.ObjectID))
            {
                logger.LogData($"Updating flight {Id} {nameof(CrewAsIDs)}: {updateData.ObjectID} => {updateData.NewObjectID}");
                crewAsIDsUpdated = CrewAsIDs.Where(id => id != updateData.ObjectID).Concat([updateData.NewObjectID]).ToArray();
            }
            if (LoadAsIDs.Contains(updateData.ObjectID))
            {
                logger.LogData($"Updating flight {Id} {nameof(LoadAsIDs)}: {updateData.ObjectID} => {updateData.NewObjectID}");
                loadAsIDsUpdated = LoadAsIDs.Where(id => id != updateData.ObjectID).Concat([updateData.NewObjectID]).ToArray();
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
                latitudeUpdated = updateData.Latitude;
                longitudeUpdated = updateData.Longitude;
            }
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

        private static Dictionary<string, Action<FlightUpdateDecorator, string>> CreateAddDictionary() =>
            new()
            {
                {QuerySyntax.Flight.IdField, (flight, value) => flight.Id = ulong.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.Flight.AmslField, (flight, value) => flight.AMSL = ulong.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.Flight.LandingTimeField, (flight, value) => flight.LandingTime = DateTime.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.Flight.OriginField, (flight, value) => flight.OriginAsID = ulong.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.Flight.PlaneField, (flight, value) => flight.PlaneID = ulong.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.Flight.TakeofTimeField, (flight, value) => flight.TakeoffTime = DateTime.Parse(value, CultureInfo.CurrentUICulture) },
                {QuerySyntax.Flight.TargetField, (flight, value) => flight.TargetAsID = ulong.Parse(value, CultureInfo.CurrentUICulture) },
                {$"{QuerySyntax.Airport.WorldPositionField}.{QuerySyntax.WorldPosition.LongitudeField}", (airport, value) => airport.Longitude = float.Parse(value, CultureInfo.CurrentUICulture) },
                {$"{QuerySyntax.Airport.WorldPositionField}.{QuerySyntax.WorldPosition.LatitudeField}", (airport, value) => airport.Latitude = float.Parse(value, CultureInfo.CurrentUICulture) },
            };
    }

}
