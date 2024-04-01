using FlightApp;
using FlightApp.DataProcessor;
using FlightApp.News;
using FlightTrackerGUI;
using Mapsui.Projections;

internal class FlightAppLogic: IDisposable
{
	private readonly IFlightAppDataProcessor dataProcessor;
	private readonly Timer timer;
	private bool disposedValue;

	public FlightAppLogic(IFlightAppDataProcessor flightAppDataProcessor)
	{
		dataProcessor = flightAppDataProcessor;
		timer = new Timer(new TimerCallback(MapRefresh), null, Timeout.Infinite, Timeout.Infinite);
	}

	public void StartNetworkSimulator()
	{
		dataProcessor.Start();
		Thread mapThread = new Thread(() => Runner.Run());
		mapThread.IsBackground = true;
		mapThread.Start();
		timer.Change(1000, 1000);
	}

	public void MakeSnapshot()
	{
		var snapshotFileName = $"snapshot_{DateTime.Now:HH_mm_ss}.json";
		IDataSerializer serializer = new DataJsonSerializer();
		var data = dataProcessor.FlightAppCompleteData.GetCompleteData();
		serializer.Serialize(data, snapshotFileName);
	}

	public IEnumerable<string> Report()
	{
		var generator = new NewsGenerator(
			new INewsReporter[] {
				new Televison("Telewizja Abelowa"),
				new Televison("Kanał TV-tensor"),
				new Radio("Radio Kwantyfikator"),
				new Radio("Radio Shmem"),
				new Newspaper("Gazeta Kategoryczna"),
				new Newspaper("Dziennik Politechniczny"),
			},
			[
			.. dataProcessor.FlightAppCompleteData.GetAirports().Values,
			.. dataProcessor.FlightAppCompleteData.GetCargoPlanes(),
			.. dataProcessor.FlightAppCompleteData.GetPassangerPlanes()
			]);

		string? info;
		do
		{
			info = generator.GenerateNextNews();
			if (info != null)
			{
				yield return info;
			}
		}
		while (info != null);
	}

	private void MapRefresh(object? state)
	{
		var sourceFlights = dataProcessor.FlightAppCompleteData.GetFlights();
		var sourceAirports = dataProcessor.FlightAppCompleteData.GetAirports();
		List<FlightGUI> flights = new List<FlightGUI>();

		foreach (var flightMapInfo in sourceFlights.Select(f => GetFlightMapInfo(f, sourceAirports)).Where(fi => fi.isLive))
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

	private (Flight flight, bool isLive, WorldPosition position, double rotation) GetFlightMapInfo(Flight flight, IReadOnlyDictionary<ulong, Airport> airports)
	{
		var displayTime = DateTime.Now.TimeOfDay;
		var isLive = false;

		if (flight.TakeoffTime.TimeOfDay < flight.LandingTime.TimeOfDay)
		{
			isLive = flight.TakeoffTime.TimeOfDay < displayTime && displayTime < flight.LandingTime.TimeOfDay;
		}
		else if (flight.TakeoffTime.TimeOfDay > flight.LandingTime.TimeOfDay)
		{
			isLive = flight.TakeoffTime.TimeOfDay < displayTime || displayTime < flight.LandingTime.TimeOfDay;
		}

		if (isLive && airports.TryGetValue(flight.OriginAsID, out var originAirport) && airports.TryGetValue(flight.TargetAsID, out var targetAirport))
		{
			var worldPosition = CalculateFlightPosition(flight, originAirport, targetAirport, displayTime);
			var rotation = CalculateFlightRotation(flight, originAirport, targetAirport);

			return (flight, isLive, worldPosition, rotation);
		}

		return (flight, isLive, new WorldPosition(0, 0), 0);

	}

	private double CalculateFlightRotation(Flight flight, Airport originAirport, Airport targetAirport)
	{
		(double x_origin, double y_origin) = SphericalMercator.FromLonLat(originAirport.Longitude, originAirport.Latitude);
		(double x_target, double y_target) = SphericalMercator.FromLonLat(targetAirport.Longitude, targetAirport.Latitude);
		double num = Math.PI / 2 - Math.Atan2(y_target - y_origin, x_target - x_origin); 
		if (num <= 0.0)
		{
			return 2 * Math.PI + num;
		}
		return num;
	}

	private WorldPosition CalculateFlightPosition(Flight flight, Airport originAirport, Airport targetAirport, TimeSpan timeOfDay)
	{
		if (flight.Longitude.HasValue && flight.Latitude.HasValue)
		{
			return new WorldPosition(flight.Latitude.Value, flight.Longitude.Value);
		}

		var flightDuracy = flight.LandingTime.TimeOfDay - flight.TakeoffTime.TimeOfDay;
		if (flightDuracy < TimeSpan.Zero)
		{
			flightDuracy += TimeSpan.FromDays(1);
		}

		var flightTime = timeOfDay - flight.TakeoffTime.TimeOfDay;
		if (flightTime < TimeSpan.Zero)
		{
			flightTime += TimeSpan.FromDays(1);
		}

		var flightCompletePart = flightTime / flightDuracy;

		var x = originAirport.Latitude + (targetAirport.Latitude - originAirport.Latitude) * flightCompletePart;
		var y = originAirport.Longitude + (targetAirport.Longitude - originAirport.Longitude) * flightCompletePart;

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
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

}
