using FlightApp.Readers;

namespace FlightApp.DataProcessor;

internal class FtrDataProcessor : IFlightAppDataProcessor
{
    private readonly IFlightAppObjectFtrReader reader;
    private readonly string dataPath;

    public FtrDataProcessor(string dataFilePath, IFlightAppObjectFtrReader flightAppObjectFtrReader, IFlightAppCompleteData flightAppCompleteData)
    {
        dataPath = dataFilePath;
        FlightAppCompleteData = flightAppCompleteData;
        reader = flightAppObjectFtrReader;
    }

    public IFlightAppCompleteData FlightAppCompleteData { get; }

    public void Start()
    {
        if (!File.Exists(dataPath))
        {
            throw new FileNotFoundException();
        }

        Load(File.OpenRead(dataPath));
    }

    private void Load(Stream data)
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
                    this.reader.AddToFlightAppCompleteData(line.Split(","), FlightAppCompleteData);
                }
            }
        }
    }
}
