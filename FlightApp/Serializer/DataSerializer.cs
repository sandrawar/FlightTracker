using System.Text.Json;

namespace FlightApp.Serializer;

internal interface IDataSerializer
{
    string Serialize(IEnumerable<IFlightAppObject> data, string filePath);
}

internal class DataJsonSerializer : IDataSerializer
{
    public string Serialize(IEnumerable<IFlightAppObject> data, string filePath)
    {
        using (var file = File.Create(filePath))
        {
            Serialize(data, file);
            return file.Name;
        }
    }

    private void Serialize(IEnumerable<IFlightAppObject> data, Stream output)
    {
        JsonSerializer.Serialize(output, data, new JsonSerializerOptions() { 
            WriteIndented = true,
            Converters = { new SerializeConverter() },
        });
    }
}
