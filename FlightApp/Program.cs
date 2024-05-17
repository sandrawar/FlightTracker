using FlightApp.Commands;
using FlightApp.DataProcessor;
using FlightApp.Logger;
using FlightApp.Query;

internal class Program
{
    private static void Main(string[] args)
    {
        var logger = new Logger();
        IFlightAppCompleteData flightAppCompleteData = new FlightAppCompleteData(logger);
        IFlightAppDataProcessor dataProcessor = new DataProcessorFactory().Create(args[0], args[1], flightAppCompleteData);
        IFlighAppQueryProcessor flighAppQueryProcessor = new FlighAppQueryProcessor(flightAppCompleteData);
        IFlighAppCommandProcessor flighAppCommandProcessor = new FlighAppCommandProcessor(flightAppCompleteData, flighAppQueryProcessor);

        var logic = new FlightAppLogic(flightAppCompleteData, flighAppCommandProcessor);

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

                    default:
                        if (command is { })
                        {
                            var result = logic.ProcessCommand(command);
                            foreach (var info in result.Messages)
                            {
                                Console.WriteLine(info);
                            }
                        }
                        break;
                }
            }
        }
        finally
        {
            logic.Dispose();
            logger.Dispose();
        }

        void PrintManual()
        {
            Console.WriteLine(string.Empty);
            Console.WriteLine("Flight App commands:");
            Console.WriteLine("  exit - close application");
            Console.WriteLine("  print - create data snapshot");
            Console.WriteLine("  report - create news report");
            Console.WriteLine("  display {object_fields} from {object_class} [where {conditions}]");
            Console.WriteLine("  update (object_class} set ({key_value_List}) [where {conditions}]");
            Console.WriteLine("  delete {object_class} [where {conditions}]");
            Console.WriteLine("  add {object_class} new ({key_value_List})");
        }
    }

}