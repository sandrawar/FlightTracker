﻿using FlightApp;
using FlightTrackerGUI;
using Mapsui.Projections;
using NetworkSourceSimulator;

public class FlightAppLogic: IDisposable
{
	private readonly IFlightAppCompleteData flightAppCompleteData;
	private readonly IDataSource simulator;
	private readonly IFlightAppBinaryMessageReader messageReader;
	private readonly Timer timer;
	private bool disposedValue;
	private WorldPosition worldPosition;

	public FlightAppLogic(string dataFilePath)
	{
		flightAppCompleteData = new FlightAppCompleteData();
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
		var data = flightAppCompleteData.GetCompleteData();
		serializer.Serialize(data, snapshotFileName);
	}

	private void MapRefresh(object? state)
	{
		var sourceFlights = flightAppCompleteData.GetFlights();
		var sourceAirports = flightAppCompleteData.GetAirports();
		List<FlightGUI> flights = new List<FlightGUI>();
		worldPosition.Latitude += 1;
		worldPosition.Longitude += 1;

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

	private void Simulator_OnNewDataReady(object sender, NewDataReadyArgs args)
	{
		var message = simulator.GetMessageAt(args.MessageIndex);
		messageReader.AddToFlightAppCompleteData(message.MessageBytes, flightAppCompleteData);
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
