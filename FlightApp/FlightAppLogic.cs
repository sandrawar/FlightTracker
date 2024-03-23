using FlightApp;
using FlightTrackerGUI;
using Mapsui.Projections;
using NetworkSourceSimulator;
using System;
using System.Collections.Concurrent;

public class FlightAppLogic: IDisposable
{
	private readonly ConcurrentDictionary<ulong, FlightAppObject> data;
	private readonly IDataSource simulator;
	private readonly IFlightAppBinaryMessageReader messageReader;
	private readonly Timer timer;
	private bool disposedValue;
	private WorldPosition worldPosition;

	public FlightAppLogic(string dataFilePath)
	{
		data = new();
		messageReader = new FlightAppBinaryMessageReader();
		simulator = new NetworkSourceSimulator.NetworkSourceSimulator(dataFilePath, 1, 5);
		simulator.OnNewDataReady += Simulator_OnNewDataReady;
		timer = new Timer(new TimerCallback(MapRefresh), null, Timeout.Infinite, Timeout.Infinite);
	}

	public void StartNetworkSimulator()
	{
		Thread simulateThread = new Thread(() => simulator.Run());
		simulateThread.IsBackground = true;
		simulateThread.Start();
		Thread mapThread = new Thread(() => Runner.Run());
		mapThread.IsBackground = true;
		mapThread.Start();
		timer.Change(1000, 1000);
	}

	public void MakeSnapshot()
	{
		var snapshotFileName = $"snapshot_{DateTime.Now:HH_mm_ss}.json";
		IDataSerializer serializer = new DataJsonSerializer();
		serializer.Serialize(data.Values.ToArray(), snapshotFileName);
	}

	private void MapRefresh(object? state)
	{
		emulatedTime += TimeSpan.FromMinutes(10);
		var sourceData = data.Values.ToArray();
		List<FlightGUI> flights = new List<FlightGUI>();
		worldPosition.Latitude += 1;
		worldPosition.Longitude += 1;
		/*flights.Add(new FlightGUI()
		{
			ID = 1,
			MapCoordRotation = 0,
			WorldPosition = worldPosition,
		});*/

		foreach (var flight in sourceData.Where(d => d is Flight).Cast<Flight>().Where(IsFlightLive))
		{
			var position = CalculateFlightPosition(flight);
			var rotation = CalculateFlightRotation(flight);
			if (position.HasValue) {
				flights.Add(new FlightGUI()
				{
					ID = flight.Id,
					MapCoordRotation = rotation,
					WorldPosition = position.Value,
				});
			}
		}
		var flightsGUIData = new FlightsGUIData(flights);
		Runner.UpdateGUI(flightsGUIData);
	}

	private DateTime emulatedTime = DateTime.Today;

	private DateTime currentTime => emulatedTime; // DateTime.Now;  

	private bool IsFlightLive(Flight flight)
	{
		var now = currentTime.TimeOfDay;
		return flight.TakeoffTime.TimeOfDay < now && flight.LandingTime.TimeOfDay > now;
	}


	private double CalculateFlightRotation(Flight flight)
	{
		if (data.TryGetValue(flight.OriginAsID, out var origin) && data.TryGetValue(flight.TargetAsID, out var target))
		{
			var originAirport = origin as Airport;
			var targetAirport = target as Airport;
			if (originAirport != null && targetAirport != null)
			{
				(double x_origin, double y_origin) = SphericalMercator.FromLonLat(originAirport.Longitude, originAirport.Latitude);
				(double x_target, double y_target) = SphericalMercator.FromLonLat(targetAirport.Longitude, targetAirport.Latitude);
				double num = Math.PI/2 - Math.Atan2(y_target - y_origin, x_target - x_origin); //- Math.PI / 2.0;
				if (num < 0.0)
				{
					return 2 * Math.PI + num;
				}
				return num;
			}

			/*
			var originAirport = origin as Airport;
			var targetAirport = target as Airport;
			if (originAirport != null && targetAirport != null)
			{
				(double x_origin, double y_origin) = SphericalMercator.FromLonLat(originAirport.Latitude, originAirport.Longitude);

				(double x_target, double y_target) = SphericalMercator.FromLonLat(targetAirport.Latitude, targetAirport.Longitude);

				return Math.PI/2 - Math.Atan((targetAirport.Latitude - originAirport.Latitude) / (targetAirport.Longitude - originAirport.Longitude));
				//return Math.Atan((y_target - y_origin) / (x_target - x_origin));
				
				double atan = Math.Atan((targetAirport.Latitude - originAirport.Latitude) / (targetAirport.Longitude - originAirport.Longitude));
				return Math.Atan((y_target - y_origin) / (x_target - x_origin));            

				if (atan > 0)
				{
					return Math.PI / 2 - atan;
				}
				return 2 * Math.PI + atan;*/
			//}
		}
		return 0;
	}


	private WorldPosition? CalculateFlightPosition(Flight flight)
	{
		if (flight.Longitude.HasValue && flight.Latitude.HasValue)
		{
			return new WorldPosition(flight.Latitude.Value, flight.Longitude.Value);
		}
		if (data.TryGetValue(flight.OriginAsID, out var origin) && data.TryGetValue(flight.TargetAsID, out var target))
		{
			var originAirport = origin as Airport;
			var targetAirport = target as Airport;
			if (originAirport != null && targetAirport != null)
			{
				var flightCompletePart = (currentTime.TimeOfDay - flight.TakeoffTime.TimeOfDay) / (flight.LandingTime.TimeOfDay - flight.TakeoffTime.TimeOfDay);
				
				var x = originAirport.Latitude + (targetAirport.Latitude - originAirport.Latitude) * flightCompletePart;
				var y = originAirport.Longitude + (targetAirport.Longitude - originAirport.Longitude) * flightCompletePart;

				(double x_target, double y_target) = SphericalMercator.FromLonLat(targetAirport.Longitude, targetAirport.Latitude);
				(double x_origin, double y_origin) = SphericalMercator.FromLonLat(originAirport.Longitude, originAirport.Latitude);
				//var x = x_origin + (x_target - x_origin) * flightCompletePart;
				//var y = y_origin + (y_target - y_origin) * flightCompletePart;
				return new WorldPosition(x, y);
			}
		}
		return null;
	}

	private void Simulator_OnNewDataReady(object sender, NewDataReadyArgs args)
	{
		var message = simulator.GetMessageAt(args.MessageIndex);
		var flightAppObject = messageReader.Read(message.MessageBytes);
		data[flightAppObject.Id] = flightAppObject;
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
