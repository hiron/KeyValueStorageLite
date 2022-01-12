using System.Text.Json;
using System.Text.Json.Serialization;

namespace KeyValueStorageLite
{
    public class KeyValueItemSystemTextJsonSerializer : IKeyValueItemSerializer
    {
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
        };

        public T? GetValue<T>(string stringValue)
        {
            if (stringValue == null)
                return default;

            return JsonSerializer.Deserialize<T>(stringValue, _jsonOptions);
        }

        public string? SerializeToString(object value)
        {
            if (value == null)
                return null;
            return JsonSerializer.Serialize(value, _jsonOptions);
        }
    }
}
