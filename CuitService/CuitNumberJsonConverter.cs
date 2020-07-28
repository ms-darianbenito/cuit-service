using System.Text.Json;
using System.Text.Json.Serialization;

namespace CuitService
{
    public class CuitNumberJsonConverter : JsonConverter<CuitNumber>
    {
        public override CuitNumber Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
        {
            return new CuitNumber(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, CuitNumber value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.FormattedValue);
        }
    }
}
