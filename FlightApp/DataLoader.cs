namespace FlightApp;

internal interface IDataLoader
{
    void Load(Stream data, IFlightAppCompleteData flightAppCompleteData);
}

internal class FtrDataLoader : IDataLoader
{
    private readonly IFlightAppObjectFtrReader factory;

    public FtrDataLoader()
    {
        factory = new FlightAppFtrReader();  
    }

    public void Load(string filePath, IFlightAppCompleteData flightAppCompleteData)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException();
        }

        Load(File.OpenRead(filePath), flightAppCompleteData);
    }

    public void Load(Stream data, IFlightAppCompleteData flightAppCompleteData)
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
