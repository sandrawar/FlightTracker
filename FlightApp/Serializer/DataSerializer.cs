using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlightApp.Serializer;

internal interface IDataSerializer
{
    void Serialize(IEnumerable<IFlightAppObject> data, string filePath);
}

internal class DataJsonSerializer : IDataSerializer
{
    public void Serialize(IEnumerable<IFlightAppObject> data, string filePath)
    {
        using (var file = File.Create(filePath))
        {
            Serialize(data, file);
        }
    }

    public void Serialize(IEnumerable<IFlightAppObject> data, Stream output)
    {
        JsonSerializer.Serialize(output, data, new JsonSerializerOptions() { 
            WriteIndented = true,
            Converters = { new SerializeConverter() },
        });
    }
}
