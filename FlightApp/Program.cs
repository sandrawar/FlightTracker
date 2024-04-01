
using FlightApp;
using FlightApp.DataProcessor;

internal class Program
{
    private static bool useSimulator = false;
    private static void Main(string[] args)
    {
        PrintManual();

        IFlightAppDataProcessor dataProcessor = useSimulator
            ? new NetworkSimulatorDataProcessor(args[0])
            : new FtrDataProcessor(args[0]);

        var logic = new FlightAppLogic(dataProcessor);

        try
        {

            logic.StartNetworkSimulator();

            bool endApplication = false;
            while (!endApplication)
            {
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
                        PrintManual();
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
            Console.WriteLine("Flight App commands:");
            Console.WriteLine("  exit - close application");
            Console.WriteLine("  print - create data snapshot");
            Console.WriteLine("  report - create news report");
        }
    }

}