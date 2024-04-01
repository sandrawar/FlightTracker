using System.Collections.Concurrent;

namespace FlightApp
{
    internal interface IFlightAppCompleteData
    {
        void Add(Airport airport);
        void Add(Cargo cargo);
        void Add(CargoPlane cargoPlane);
        void Add(Crew crew);
        void Add(Flight flight);
        void Add(Passanger passanger);
        void Add(PassangerPlane passangerPlane);

        IReadOnlyCollection<FlightAppObject> GetCompleteData();

        IReadOnlyDictionary<ulong, Airport> GetAirports();
        IReadOnlyCollection<CargoPlane> GetCargoPlanes();
        IReadOnlyCollection<Cargo> GetCargos();
        IReadOnlyCollection<Crew> GetCrewMembers();
        IReadOnlyCollection<Flight> GetFlights();
        IReadOnlyCollection<PassangerPlane> GetPassangerPlanes();
        IReadOnlyCollection<Passanger> GetPassangers();
    }

    internal class FlightAppCompleteData : IFlightAppCompleteData
    {
        private ConcurrentDictionary<ulong, Airport> airports;
        private ConcurrentBag<Flight> flights;
        private ConcurrentBag<Crew> crewMembers;
        private ConcurrentBag<Passanger> passangers;
        private ConcurrentBag<PassangerPlane> passangerPlanes;
        private ConcurrentBag<CargoPlane> cargoPlanes;
        private ConcurrentBag<Cargo> cargos;

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

        public IReadOnlyCollection<FlightAppObject> GetCompleteData()
        {
            FlightAppObject[] data = [
            .. airports.Values,
            .. flights,
            .. crewMembers,
            .. passangers,
            .. passangerPlanes,
            .. cargoPlanes,
            .. cargoPlanes,
            ];
            return data;
        }

        public IReadOnlyDictionary<ulong, Airport> GetAirports() => airports.AsReadOnly();
        public IReadOnlyCollection<Flight> GetFlights() => flights.ToArray();
        public IReadOnlyCollection<Crew> GetCrewMembers() => crewMembers.ToArray();
        public IReadOnlyCollection<Passanger> GetPassangers() => passangers.ToArray();
        public IReadOnlyCollection<PassangerPlane> GetPassangerPlanes() => passangerPlanes.ToArray();
        public IReadOnlyCollection<CargoPlane> GetCargoPlanes() => cargoPlanes.ToArray();
        public IReadOnlyCollection<Cargo> GetCargos() => cargos.ToArray();

        public void Add(Airport airport) => airports[airport.Id] = airport;
        public void Add(Flight flight) => flights.Add(flight);
        public void Add(Crew crew) => crewMembers.Add(crew);
        public void Add(Passanger passanger) => passangers.Add(passanger);
        public void Add(PassangerPlane passangerPlane) => passangerPlanes.Add(passangerPlane);
        public void Add(Cargo cargo) => cargos.Add(cargo);
        public void Add(CargoPlane cargoPlane) => cargoPlanes.Add(cargoPlane);
    }
}
