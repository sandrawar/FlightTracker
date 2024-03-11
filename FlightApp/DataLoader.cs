namespace FlightApp;

internal interface IDataLoader
{
    IEnumerable<FlightAppObject> Load(string filePath);
}

internal class FtrDataLoader : IDataLoader
{
    private readonly IFlightAppFtrReader factory;

    public FtrDataLoader()
    {
        factory = new FlightAppFtrReader();  
    }

    public IEnumerable<FlightAppObject> Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException();
        }

        return Load(File.OpenRead(filePath));
    }

    public IEnumerable<FlightAppObject> Load(Stream data)
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
                    yield return factory.Create(line.Split(","));
                }
            }
        }
    }
}
