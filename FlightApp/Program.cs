
using FlightApp;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Flight App");

        var dataFilePath = args[0];
        var outFilePath = dataFilePath + "_out.json";

        Console.WriteLine($"  data file: {dataFilePath}");
        IDataLoader loader = new FtrDataLoader();
        var data = loader.Load(dataFilePath).ToArray();
        Console.WriteLine($"  data loaded");

        Console.WriteLine($"  output file: {outFilePath}");
        IDataSerializer serializer = new DataJsonSerializer();
        serializer.Serialize(data, outFilePath);
        Console.WriteLine($"  data serialized");
    }
}