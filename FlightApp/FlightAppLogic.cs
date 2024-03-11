using FlightApp;
using System;

public class FlightAppLogic
{
	private readonly List<FlightAppObject> data;
	public FlightAppLogic()
	{
		data = new();
	}

	public void StartNetworkSimulator(string dataFilePath)
	{
        IDataLoader loader = new FtrDataLoader();
        data.AddRange(loader.Load(dataFilePath).ToArray());
    }

	public void MakeSnapshot()
	{
		var snapshotFileName = $"snapshot_{DateTime.Now:HH_mm_ss}.json";
        IDataSerializer serializer = new DataJsonSerializer();
        serializer.Serialize(data, snapshotFileName);
    }
}
