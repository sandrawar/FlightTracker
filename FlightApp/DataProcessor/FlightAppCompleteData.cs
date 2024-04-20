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

    internal interface IFlightAppCompleteData: IFlightAppDataUpdate, IFlightAppDataRead
    {
    }

    internal class FlightAppCompleteData : IFlightAppCompleteData
    {
        private ConcurrentBag<IAirportUpdateDecorator> airports;
        private ConcurrentBag<IFlightUpdateDecorator> flights;
        private ConcurrentBag<ICrewUpdateDecorator> crewMembers;
        private ConcurrentBag<IPassangerUpdateDecorator> passangers;
        private ConcurrentBag<IPassangerPlaneUpdateDecorator> passangerPlanes;
        private ConcurrentBag<ICargoPlaneUpdateDecorator> cargoPlanes;
        private ConcurrentBag<ICargoUpdateDecorator> cargos;

        public FlightAppCompleteData()
        {
            airports = new();
            flights = new();
            crewMembers = new();
            passangers = new();
            passangerPlanes = new();
            cargoPlanes = new();
            cargos = new();
        }

        public IReadOnlyCollection<IFlightAppObject> GetCompleteData()
        {
            IFlightAppObject[] data = [
            .. airports.ToArray(),
            .. flights.ToArray(),
            .. crewMembers.ToArray(),
            .. passangers.ToArray(),
            .. passangerPlanes.ToArray(),
            .. cargoPlanes.ToArray(),
            .. cargoPlanes.ToArray(),
            ];
            return data;
        }

        public IReadOnlyCollection<IAirport> GetAirports() => airports.ToArray();
        public IReadOnlyCollection<IFlight> GetFlights() => flights.ToArray();
        public IReadOnlyCollection<ICrew> GetCrewMembers() => crewMembers.ToArray();
        public IReadOnlyCollection<IPassanger> GetPassangers() => passangers.ToArray();
        public IReadOnlyCollection<IPassangerPlane> GetPassangerPlanes() => passangerPlanes.ToArray();
        public IReadOnlyCollection<ICargoPlane> GetCargoPlanes() => cargoPlanes.ToArray();
        public IReadOnlyCollection<ICargo> GetCargos() => cargos.ToArray();

        public void Add(IAirport airport) => airports.Add(new AirportUpdateDecorator(airport));
        public void Add(IFlight flight) => flights.Add(new FlightUpdateDecorator(flight));
        public void Add(ICrew crew) => crewMembers.Add(new CrewUpdateDecorator(crew));
        public void Add(IPassanger passanger) => passangers.Add(new PassangerUpdateDecorator(passanger));
        public void Add(IPassangerPlane passangerPlane) => passangerPlanes.Add(new PassangerPlaneUpdateDecorator(passangerPlane));
        public void Add(ICargo cargo) => cargos.Add(new CargoUpdateDecorator(cargo));
        public void Add(ICargoPlane cargoPlane) => cargoPlanes.Add(new CargoPlaneUpdateDecorator(cargoPlane));

        public void UpdateData(IDUpdateData iDUpdateData)
        {
        }

        public void UpdateData(PositionUpdateData positionUpdateData)
        {
        }

        public void UpdateData(ContactInfoUpdateData contactInfoUpdateData)
        {
        }
    }
}
