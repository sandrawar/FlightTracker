using FlightApp.Logger;
using System.Collections.Concurrent;

namespace FlightApp.DataProcessor
{
    internal interface IFlightAppDataUpdate
    {
        void Add(IAirport airport);
        void Add(ICargo cargo);
        void Add(ICargoPlane cargoPlane);
        void Add(ICrew crew);
        void Add(IFlight flight);
        void Add(IPassanger passanger);
        void Add(IPassangerPlane passangerPlane);

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
        IReadOnlyCollection<IPassangerPlane> GetPassangerPlanes();
        IReadOnlyCollection<IPassanger> GetPassangers();
    }

    internal interface IFlightAppCompleteData : IFlightAppDataUpdate, IFlightAppDataRead
    {
    }

    internal class FlightAppCompleteData : IFlightAppCompleteData
    {
        private readonly ReaderWriterLockSlim locker;

        private List<IAirportUpdateDecorator> airports;
        private List<IFlightUpdateDecorator> flights;
        private List<ICrewUpdateDecorator> crewMembers;
        private List<IPassangerUpdateDecorator> passangers;
        private List<IPassangerPlaneUpdateDecorator> passangerPlanes;
        private List<ICargoPlaneUpdateDecorator> cargoPlanes;
        private List<ICargoUpdateDecorator> cargos;

        private readonly ILogger updateLogger;

        public FlightAppCompleteData(ILogger logger)
        {
            locker = new ReaderWriterLockSlim();

            airports = new();
            flights = new();
            crewMembers = new();
            passangers = new();
            passangerPlanes = new();
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
            .. passangers.ToArray(),
            .. passangerPlanes.ToArray(),
            .. cargoPlanes.ToArray(),
            .. cargoPlanes.ToArray(),
            ]);

        public IReadOnlyCollection<IAirport> GetAirports() => ReadOperation(() => airports.ToArray());
        public IReadOnlyCollection<IFlight> GetFlights() => ReadOperation(() => flights.ToArray());
        public IReadOnlyCollection<ICrew> GetCrewMembers() => ReadOperation(() => crewMembers.ToArray());
        public IReadOnlyCollection<IPassanger> GetPassangers() => ReadOperation(() => passangers.ToArray());
        public IReadOnlyCollection<IPassangerPlane> GetPassangerPlanes() => ReadOperation(() => passangerPlanes.ToArray());
        public IReadOnlyCollection<ICargoPlane> GetCargoPlanes() => ReadOperation(() => cargoPlanes.ToArray());
        public IReadOnlyCollection<ICargo> GetCargos() => ReadOperation(() => cargos.ToArray());

        public void Add(IAirport airport) => WriteOperation(() => airports.Add(new AirportUpdateDecorator(airport)));
        public void Add(IFlight flight) => WriteOperation(() => flights.Add(new FlightUpdateDecorator(flight)));
        public void Add(ICrew crew) => WriteOperation(() => crewMembers.Add(new CrewUpdateDecorator(crew)));
        public void Add(IPassanger passanger) => WriteOperation(() => passangers.Add(new PassangerUpdateDecorator(passanger)));
        public void Add(IPassangerPlane passangerPlane) => WriteOperation(() => passangerPlanes.Add(new PassangerPlaneUpdateDecorator(passangerPlane)));
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
                        passangers.ForEach(p => p.UpdateData(iDUpdateData, updateLogger));
                        passangerPlanes.ForEach(pp => pp.UpdateData(iDUpdateData, updateLogger));
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
                passangers.ForEach(p => p.UpdateData(contactInfoUpdateData, updateLogger));
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
    }
}
