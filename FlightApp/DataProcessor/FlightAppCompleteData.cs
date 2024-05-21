using FlightApp.Logger;

namespace FlightApp.DataProcessor
{
    internal interface IFlightAppDataUpdate
    {
        void Add(IAirport airport);
        void Add(ICargo cargo);
        void Add(ICargoPlane cargoPlane);
        void Add(ICrew crew);
        void Add(IFlight flight);
        void Add(IPassenger passenger);
        void Add(IPassengerPlane passengerPlane);

        void UpdateData(IDUpdateData iDUpdateData);
        void UpdateData(PositionUpdateData positionUpdateData);
        void UpdateData(ContactInfoUpdateData contactInfoUpdateData);
    }

    internal interface IFlightAppDataRead
    {
        IReadOnlyCollection<IFlightAppObject> GetCompleteData();

        IReadOnlyCollection<IAirport> GetAirports();
        IReadOnlyCollection<ICargoPlane> GetCargoPlanes();
        IReadOnlyCollection<ICargo> GetCargos();
        IReadOnlyCollection<ICrew> GetCrewMembers();
        IReadOnlyCollection<IFlight> GetFlights();
        IReadOnlyCollection<IPassengerPlane> GetPassengerPlanes();
        IReadOnlyCollection<IPassenger> GetPassengers();
    }

    internal interface IFlightAppDataQueryRepository : IFlightAppDataUpdate
    {
        IReadOnlyCollection<IAirportUpdateDecorator> GetAirportsUpdate();
        IReadOnlyCollection<ICargoPlaneUpdateDecorator> GetCargoPlanesUpdate();
        IReadOnlyCollection<ICargoUpdateDecorator> GetCargosUpdate();
        IReadOnlyCollection<ICrewUpdateDecorator> GetCrewMembersUpdate();
        IReadOnlyCollection<IFlightUpdateDecorator> GetFlightsUpdate();
        IReadOnlyCollection<IPassengerPlaneUpdateDecorator> GetPassengerPlanesUpdate();
        IReadOnlyCollection<IPassengerUpdateDecorator> GetPassengersUpdate();

        bool Delete(IAirportUpdateDecorator airport);
        bool Delete(ICargoUpdateDecorator cargo);
        bool Delete(ICargoPlaneUpdateDecorator cargoPlane);
        bool Delete(ICrewUpdateDecorator crew);
        bool Delete(IFlightUpdateDecorator flight);
        bool Delete(IPassengerUpdateDecorator passenger);
        bool Delete(IPassengerPlaneUpdateDecorator passengerPlane);
    }

    internal interface IFlightAppCompleteData : IFlightAppDataRead, IFlightAppDataUpdate, IFlightAppDataQueryRepository
    {
    }

    internal class FlightAppCompleteData : IFlightAppCompleteData
    {
        private readonly ReaderWriterLockSlim locker;

        private List<IAirportUpdateDecorator> airports;
        private List<IFlightUpdateDecorator> flights;
        private List<ICrewUpdateDecorator> crewMembers;
        private List<IPassengerUpdateDecorator> passengers;
        private List<IPassengerPlaneUpdateDecorator> passengerPlanes;
        private List<ICargoPlaneUpdateDecorator> cargoPlanes;
        private List<ICargoUpdateDecorator> cargos;

        private readonly ILogger updateLogger;

        public FlightAppCompleteData(ILogger logger)
        {
            locker = new ReaderWriterLockSlim();

            airports = new();
            flights = new();
            crewMembers = new();
            passengers = new();
            passengerPlanes = new();
            cargoPlanes = new();
            cargos = new();
            updateLogger = logger;
        }

        public IReadOnlyCollection<IFlightAppObject> GetCompleteData()
            => ReadOperation<IReadOnlyCollection<IFlightAppObject>>(() =>
            [
            .. airports.ToArray(),
            .. flights.ToArray(),
            .. crewMembers.ToArray(),
            .. passengers.ToArray(),
            .. passengerPlanes.ToArray(),
            .. cargoPlanes.ToArray(),
            .. cargoPlanes.ToArray(),
            ]);

        public IReadOnlyCollection<IAirport> GetAirports() => ReadOperation(airports.ToArray);
        public IReadOnlyCollection<IFlight> GetFlights() => ReadOperation(flights.ToArray);
        public IReadOnlyCollection<ICrew> GetCrewMembers() => ReadOperation(crewMembers.ToArray);
        public IReadOnlyCollection<IPassenger> GetPassengers() => ReadOperation(passengers.ToArray);
        public IReadOnlyCollection<IPassengerPlane> GetPassengerPlanes() => ReadOperation(passengerPlanes.ToArray);
        public IReadOnlyCollection<ICargoPlane> GetCargoPlanes() => ReadOperation(cargoPlanes.ToArray);
        public IReadOnlyCollection<ICargo> GetCargos() => ReadOperation(cargos.ToArray);

        public void Add(IAirport airport) => WriteOperation(() => airports.Add(new AirportUpdateDecorator(airport)));
        public void Add(IFlight flight) => WriteOperation(() => flights.Add(new FlightUpdateDecorator(flight)));
        public void Add(ICrew crew) => WriteOperation(() => crewMembers.Add(new CrewUpdateDecorator(crew)));
        public void Add(IPassenger passenger) => WriteOperation(() => passengers.Add(new PassengerUpdateDecorator(passenger)));
        public void Add(IPassengerPlane passengerPlane) => WriteOperation(() => passengerPlanes.Add(new PassengerPlaneUpdateDecorator(passengerPlane)));
        public void Add(ICargo cargo) => WriteOperation(() => cargos.Add(new CargoUpdateDecorator(cargo)));
        public void Add(ICargoPlane cargoPlane) => WriteOperation(() => cargoPlanes.Add(new CargoPlaneUpdateDecorator(cargoPlane)));

        public void UpdateData(IDUpdateData iDUpdateData)
        {
            if (iDUpdateData.NewObjectID == iDUpdateData.ObjectID)
            {
                updateLogger.LogData($"Ignoring ID update data {iDUpdateData.ObjectID} => {iDUpdateData.NewObjectID}");
                return;
            }

            WriteOperation(() =>
                    {
                        airports.ForEach(a => a.UpdateData(iDUpdateData, updateLogger));
                        flights.ForEach(f => f.UpdateData(iDUpdateData, updateLogger));
                        crewMembers.ForEach(c => c.UpdateData(iDUpdateData, updateLogger));
                        passengers.ForEach(p => p.UpdateData(iDUpdateData, updateLogger));
                        passengerPlanes.ForEach(pp => pp.UpdateData(iDUpdateData, updateLogger));
                        cargoPlanes.ForEach(cp => cp.UpdateData(iDUpdateData, updateLogger));
                        cargos.ForEach(c => c.UpdateData(iDUpdateData, updateLogger));
                    });
        }

        public void UpdateData(PositionUpdateData positionUpdateData)
        {
            WriteOperation(() =>
                flights.ForEach(f => f.UpdateData(positionUpdateData, updateLogger)));
        }

        public void UpdateData(ContactInfoUpdateData contactInfoUpdateData)
            => WriteOperation(() =>
            {
                crewMembers.ForEach(c => c.UpdateData(contactInfoUpdateData, updateLogger));
                passengers.ForEach(p => p.UpdateData(contactInfoUpdateData, updateLogger));
            });

        private void WriteOperation(Action operation)
        {
            locker.EnterWriteLock();
            try
            {
                operation();
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        private TResult WriteOperation<TResult>(Func<TResult> operation)
        {
            locker.EnterWriteLock();
            try
            {
                return operation();
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        private TResult ReadOperation<TResult>(Func<TResult> operation)
        {
            locker.EnterReadLock();
            try
            {
                return operation();
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        public IReadOnlyCollection<IAirportUpdateDecorator> GetAirportsUpdate() => ReadOperation(airports.ToArray);

        public IReadOnlyCollection<ICargoPlaneUpdateDecorator> GetCargoPlanesUpdate() => ReadOperation(cargoPlanes.ToArray);

        public IReadOnlyCollection<ICargoUpdateDecorator> GetCargosUpdate() => ReadOperation(cargos.ToArray);

        public IReadOnlyCollection<ICrewUpdateDecorator> GetCrewMembersUpdate() => ReadOperation(crewMembers.ToArray);

        public IReadOnlyCollection<IFlightUpdateDecorator> GetFlightsUpdate() => ReadOperation(flights.ToArray);

        public IReadOnlyCollection<IPassengerPlaneUpdateDecorator> GetPassengerPlanesUpdate() => ReadOperation(passengerPlanes.ToArray);

        public IReadOnlyCollection<IPassengerUpdateDecorator> GetPassengersUpdate() => ReadOperation(passengers.ToArray);

        public bool Delete(IAirportUpdateDecorator airport) => WriteOperation(() => airports.Remove(airport));

        public bool Delete(ICargoUpdateDecorator cargo) => WriteOperation(() => cargos.Remove(cargo));

        public bool Delete(ICargoPlaneUpdateDecorator cargoPlane) => WriteOperation(() => cargoPlanes.Remove(cargoPlane));

        public bool Delete(ICrewUpdateDecorator crew) => WriteOperation(() => crewMembers.Remove(crew));

        public bool Delete(IFlightUpdateDecorator flight) => WriteOperation(() => flights.Remove(flight));

        public bool Delete(IPassengerUpdateDecorator passenger) => WriteOperation(() => passengers.Remove(passenger));

        public bool Delete(IPassengerPlaneUpdateDecorator passengerPlane) => WriteOperation(() => passengerPlanes.Remove(passengerPlane));
    }
}
