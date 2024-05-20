using FlightApp;
using FlightApp.Command;
using FlightApp.DataProcessor;
using FlightTrackerGUI;
using Mapsui.Projections;

internal class FlightAppLogic: IDisposable
{
	private readonly IFlightAppDataRead appData;
	private readonly IFlighAppCommandLibrary cmdProcessor;
	private readonly Timer timer;
	private bool disposedValue;

	public FlightAppLogic(IFlightAppDataRead flightAppData, IFlighAppCommandLibrary commandProcessor)
	{
		appData = flightAppData;
		cmdProcessor = commandProcessor;
		timer = new Timer(new TimerCallback(MapRefresh), null, Timeout.Infinite, Timeout.Infinite);
	}

	public void Start()
	{
		Thread mapThread = new Thread(() => Runner.Run());
		mapThread.IsBackground = true;
		mapThread.Start();
		timer.Change(1000, 1000);
	}

	public CommandResult ProcessCommand(string command) => cmdProcessor.Execute(command);
	
	private void MapRefresh(object? state)
	{
		var sourceFlights = appData.GetFlights();
		var sourceAirports = appData.GetAirports().ToDictionary(c => c.Id, c => c);
		List<FlightGUI> flights = new List<FlightGUI>();

		foreach (var flightMapInfo in sourceFlights.Select(f => GetFlightMapInfo(f, sourceAirports)))
		{
			flights.Add(new FlightGUI()
			{
				ID = flightMapInfo.flight.Id,
				MapCoordRotation = flightMapInfo.rotation,
				WorldPosition = flightMapInfo.position,
			});
		}
		var flightsGUIData = new FlightsGUIData(flights);
		Runner.UpdateGUI(flightsGUIData);
	}

	private (IFlight flight, bool isLive, WorldPosition position, double rotation) GetFlightMapInfo(IFlight flight, IReadOnlyDictionary<ulong, IAirport> airports)
	{
		var displayTime = DateTime.Now.TimeOfDay;
		var isLive = false;

		var lastKnownPositionTime = flight.LastPositionTime ?? flight.TakeoffTime;
		if (lastKnownPositionTime.TimeOfDay < flight.LandingTime.TimeOfDay)
		{
			isLive = lastKnownPositionTime.TimeOfDay < displayTime && displayTime < flight.LandingTime.TimeOfDay;
		}
		else if (lastKnownPositionTime.TimeOfDay > flight.LandingTime.TimeOfDay)
		{
			isLive = lastKnownPositionTime.TimeOfDay < displayTime || displayTime < flight.LandingTime.TimeOfDay;
		}

		if (isLive && airports.TryGetValue(flight.OriginAsID, out var originAirport) && airports.TryGetValue(flight.TargetAsID, out var targetAirport))
		{
			var worldPosition = CalculateFlightPosition(flight, originAirport, targetAirport, displayTime);
			var rotation = CalculateFlightRotation(flight, originAirport, targetAirport);

			return (flight, isLive, worldPosition, rotation);
		}

		return (flight, isLive, new WorldPosition(500, 500), 0);

	}

	private double CalculateFlightRotation(IFlight flight, IAirport originAirport, IAirport targetAirport)
	{
		var lastKnownLatitude = flight.Latitude ?? originAirport.Latitude;
		var lastKnownLongitude = flight.Longitude ?? originAirport.Longitude;
		(double x_origin, double y_origin) = SphericalMercator.FromLonLat(lastKnownLongitude, lastKnownLatitude);
		(double x_target, double y_target) = SphericalMercator.FromLonLat(targetAirport.Longitude, targetAirport.Latitude);
		double num = Math.PI / 2 - Math.Atan2(y_target - y_origin, x_target - x_origin); 
		if (num <= 0.0)
		{
			return 2 * Math.PI + num;
		}
		return num;
	}

	private WorldPosition CalculateFlightPosition(IFlight flight, IAirport originAirport, IAirport targetAirport, TimeSpan timeOfDay)
	{
		var lastKnownPositionTime = flight.LastPositionTime ?? flight.TakeoffTime;
		var flightDuracy = flight.LandingTime.TimeOfDay - lastKnownPositionTime.TimeOfDay;
		if (flightDuracy < TimeSpan.Zero)
		{
			flightDuracy += TimeSpan.FromDays(1);
		}

		var flightTime = timeOfDay - lastKnownPositionTime.TimeOfDay;
		if (flightTime < TimeSpan.Zero)
		{
			flightTime += TimeSpan.FromDays(1);
		}

		var flightCompletePart = flightTime / flightDuracy;

		var lastKnownLatitude = flight.Latitude ?? originAirport.Latitude;
		var lastKnownLongitude = flight.Longitude ?? originAirport.Longitude;

		var x = lastKnownLatitude + (targetAirport.Latitude - lastKnownLatitude) * flightCompletePart;
		var y = lastKnownLongitude + (targetAirport.Longitude - lastKnownLongitude) * flightCompletePart;

		return new WorldPosition(x, y);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				timer.Dispose();
			}
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

}
