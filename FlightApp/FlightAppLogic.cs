using FlightApp;
using NetworkSourceSimulator;
using System.Collections.Concurrent;

public class FlightAppLogic
{
	private readonly ConcurrentBag<FlightAppObject> data;
	private readonly IDataSource simulator;
	private readonly IFlightAppBinaryMessageReader messageReader;


	public FlightAppLogic(string dataFilePath)
	{
		data = new();
		messageReader = new FlightAppBinaryMessageReader();
		simulator = new NetworkSourceSimulator.NetworkSourceSimulator(dataFilePath, 10, 50);
		simulator.OnNewDataReady += Simulator_OnNewDataReady;
	}

	public void StartNetworkSimulator()
	{
		//Thread t = new Thread(new ThreadStart(Simulate));
		//t.IsBackground = true;
		//t.Start();

		Task.Run(() =>
		{
			simulator.Run();
			Console.WriteLine("simulator end");
		});
	}

	//private void Simulate()
	//{
	//	simulator.Run();
	//}

	private void Simulator_OnNewDataReady(object sender, NewDataReadyArgs args)
	{
		var message = simulator.GetMessageAt(args.MessageIndex);
		var flightAppObject = messageReader.Read(message.MessageBytes);
		data.Add(flightAppObject);
	}

	public void MakeSnapshot()
	{
		var snapshotFileName = $"snapshot_{DateTime.Now:HH_mm_ss}.json";
		IDataSerializer serializer = new DataJsonSerializer();
		serializer.Serialize(data.ToArray(), snapshotFileName);
	}
}
