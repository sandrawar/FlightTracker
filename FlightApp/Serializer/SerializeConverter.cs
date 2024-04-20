using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlightApp.Serializer
{
    internal class SerializeConverter : JsonConverter<IFlightAppObject>
    {
        public override IFlightAppObject? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, IFlightAppObject value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case null:
                    JsonSerializer.Serialize(writer, (IFlightAppObject)null!, options);
                    break;

                default:
                    var type = value.GetType();
                    JsonSerializer.Serialize(writer, value, type, options);
                    break;
            }
        }
    }
}
