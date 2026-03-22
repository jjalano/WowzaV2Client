using System.Net.Http;

namespace WowzaV2Client.Serialization;

public interface ISerializer
{
    public HttpContent Serialize<T>(T value);
    public T? Deserialize<T>(string json);
}
