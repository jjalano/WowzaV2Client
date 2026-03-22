using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WowzaV2Client.Serialization;

public class SystemTextJsonSerializer : ISerializer
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public HttpContent Serialize<T>(T value)
    {
        return new StringContent(JsonSerializer.Serialize<T>(value, _options), Encoding.UTF8, "application/json");
    }

    public T? Deserialize<T>(string json)
    {
        if(string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(json, _options);
    }
}
