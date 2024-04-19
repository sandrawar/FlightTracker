using FlightApp.DataProcessor;
using FlightApp.Readers;

internal class Program
{
    private static bool useSimulator = false;
    private static void Main(string[] args)
    {
        IFlightAppCompleteData flightAppCompleteData = new FlightAppCompleteData();
        IFlightAppDataProcessor dataProcessor = useSimulator
            ? new NetworkSimulatorDataProcessor(args[0], new FlightAppBinaryMessageReader(), flightAppCompleteData)
            : new FtrDataProcessor(args[0], new FlightAppFtrReader(), flightAppCompleteData);

        var logic = new FlightAppLogic(flightAppCompleteData);

        try
        {
            dataProcessor.Start();
            logic.Start();

            bool endApplication = false;
            while (!endApplication)
            {
                PrintManual();
                var command = Console.ReadLine();
                switch (command)
                {
                    case "exit":
                        endApplication = true;
                        break;
                    case "print":
                        logic.MakeSnapshot();
                        Console.WriteLine("Snapshot created");
                        break;
                    case "report":
                        foreach (var info in logic.Report())
                        {
                            Console.WriteLine(info);
                        }
                        break;
                    default:
                        Console.WriteLine("Unrecognized command");
                        break;
                }
            }
        }
        finally
        {
            logic.Dispose();
        }

        void PrintManual()
        {
            Console.WriteLine(string.Empty);
            Console.WriteLine("Flight App commands:");
            Console.WriteLine("  exit - close application");
            Console.WriteLine("  print - create data snapshot");
            Console.WriteLine("  report - create news report");
        }
    }

}