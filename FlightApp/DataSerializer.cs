using System.Text.Json;

namespace FlightApp;

internal interface IDataSerializer
{
    void Serialize(IEnumerable<FlightAppObject> data, string filePath);
}

internal class DataJsonSerializer : IDataSerializer
{
    public void Serialize(IEnumerable<FlightAppObject> data, string filePath)
    {
        using (var file = File.Create(filePath))
        {
            Serialize(data, file);
        }
    }

    public void Serialize(IEnumerable<FlightAppObject> data, Stream output)
    {
        JsonSerializer.Serialize(output, data, new JsonSerializerOptions() { WriteIndented = true });
    }
}
