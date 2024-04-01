using FlightApp.Readers;

namespace FlightApp.DataProcessor;

internal class FtrDataProcessor : IFlightAppDataProcessor
{
    private readonly IFlightAppObjectFtrReader factory;
    private readonly string dataFilePath;

    public FtrDataProcessor(string dataFilePath)
    {
        this.factory = new FlightAppFtrReader();
        this.dataFilePath = dataFilePath;
    }

    public void Start(IFlightAppCompleteData flightAppCompleteData)
    {
        if (!File.Exists(dataFilePath))
        {
            throw new FileNotFoundException();
        }

        Load(File.OpenRead(dataFilePath), flightAppCompleteData);
    }

    private void Load(Stream data, IFlightAppCompleteData flightAppCompleteData)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        using (var reader = new StreamReader(data))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (!string.IsNullOrEmpty(line))
                {
                    factory.AddToFlightAppCompleteData(line.Split(","), flightAppCompleteData);
                }
            }
        }
    }
}
