
using FlightApp;

internal class Program
{
    private static void Main(string[] args)
    {
        PrintManual();

        var logic = new FlightAppLogic();

        logic.StartNetworkSimulator(args[0]);

        bool endApplication = false;
        while(!endApplication)
        {
            var command = Console.ReadLine();
            switch(command)
            {
                case "exit":
                    endApplication = true; 
                    break;
                case "print":
                    logic.MakeSnapshot();
                    Console.WriteLine("Snapshot created");
                    break;
                default:
                    Console.WriteLine("Unrecognized command");
                    PrintManual();
                    break;
            }
        }

        void PrintManual()
        {
            Console.WriteLine("Flight App commands:");
            Console.WriteLine("  exit - close application");
            Console.WriteLine("  print - create data snapshot");
        }
    }

}