using FlightApp.Readers;

namespace FlightApp.DataProcessor;

internal class FtrDataProcessor : IFlightAppDataProcessor
{
    private readonly IFlightAppObjectFtrReader reader;
    private readonly IFlightAppDataUpdate flightAppData;
    private readonly string dataPath;

    public FtrDataProcessor(string dataFilePath, IFlightAppObjectFtrReader flightAppObjectFtrReader, IFlightAppDataUpdate flightAppDataUpdate)
    {
        dataPath = dataFilePath;
        flightAppData = flightAppDataUpdate;
        reader = flightAppObjectFtrReader;
    }


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
                    this.reader.AddToFlightAppDataUpdate(line.Split(","), flightAppData);
                }
            }
        }
    }
}
