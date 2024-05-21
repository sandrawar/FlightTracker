using FlightApp;
using FlightApp.Command;
using FlightApp.DataProcessor;
using FlightApp.Logger;
using FlightApp.Query;
using FlightApp.Query.Processing;

internal class Program
{
    private static void Main(string[] args)
    {
        var logger = new Logger();
        IFlightAppCompleteData flightAppCompleteData = new FlightAppCompleteData(logger);
        IFlightAppDataProcessor dataProcessor = new DataProcessorFactory().Create(args[0], args[1], flightAppCompleteData);
        IFlightAppQueryParser flightAppQueryParser = new FlightAppQueryParser();
        IFlightAppQueryProcessor flightAppQueryProcessor = new FlightAppQueryProcessor(flightAppCompleteData);
        IFlighAppQueryLibrary flighAppQueryLibrary = new FlighAppQueryLibrary(flightAppCompleteData, flightAppQueryProcessor);
        IFlighAppCommandLibrary flighAppCommandLibrary = new FlighAppCommandLibrary(flightAppCompleteData, flightAppQueryParser, flighAppQueryLibrary);

        var logic = new FlightAppLogic(flightAppCompleteData, flighAppCommandLibrary);

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
                        if (command is not null)
                        {
                            var result = logic.ProcessCommand(command);
                            ConsoleOutput.PrintData(result.Messages);
                            if (result.QueryTable is not null)
                            {
                                ConsoleOutput.PrintTable(result.QueryTable);
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
            ConsoleOutput.PrintData([
                string.Empty,
                "Flight App commands:",
                "  exit - close application",
                "  print - create data snapshot",
                "  report - create news report",
                "  display {object_fields} from {object_class} [where {conditions}]",
                "  update (object_class} set ({key_value_List}) [where {conditions}]",
                "  delete {object_class} where {conditions}]",
                "  add {object_class} new ({key_value_List})",
            ]);
        }
    }
}