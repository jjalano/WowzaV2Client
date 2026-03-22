using System.Threading.Tasks;
using WowzaV2Client.Serialization;

namespace WowzaV2Client.Tests.Unit;


public enum StreamStatus
{
    Idle, Starting, Running, Stopping, Stopped, Error
}

public record LiveStream(string StreamName, string? Description, StreamStatus StreamStatus);

public class SerializerTests
{
    private readonly SystemTextJsonSerializer _serializer = new();

    [Fact]
    public void Serialize_SetsContentTypeToApplicationJson()
    {
        // Arrange
        var stream = new LiveStream("TestStream", "A test stream", StreamStatus.Running);
        // Act
        var content = _serializer.Serialize(stream);
        // Assert
        Assert.Equal("application/json", content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Serialize_PropertiesAreCamelCased()
    {
        // Arrange
        var stream = new LiveStream("TestStream", "A test stream", StreamStatus.Running);
        // Act
        var content = _serializer.Serialize(stream);
        var json = await content.ReadAsStringAsync();
        // Assert
        Assert.Contains("streamName", json);
        Assert.DoesNotContain("StreamName", json);
        Assert.Contains("description", json);
        Assert.DoesNotContain("Description", json);
        Assert.Contains("streamStatus", json);
        Assert.DoesNotContain("StreamStatus", json);
    }

    [Fact]
    public async Task Serialize_NullPropertiesAreOmitted()
    {
        // Arrange       
        var stream = new LiveStream("TestStream", null, StreamStatus.Running);
        // Act
        var content = _serializer.Serialize(stream);
        var json = await content.ReadAsStringAsync();
        // Assert
        Assert.DoesNotContain("description", json);

    }

    [Fact]
    public async Task Serialize_EnumSerializedAsLowercaseString()
    {
        // Arrange        
        var stream = new LiveStream("TestStream", "Test stream", StreamStatus.Running);
        // Act
        var content = _serializer.Serialize(stream);
        var json = await content.ReadAsStringAsync();
        // Assert
        Assert.DoesNotContain("2", json);
        Assert.Contains("running", json);
    }

    [Fact]
    public void Deserialize_ValidJson_ReturnsTypedObject()
    {
        // Arrange      
        var json = "{\"streamName\":\"TestStream\",\"description\":\"Test stream\",\"streamStatus\":\"running\"}";
        // Act
        var content = _serializer.Deserialize<LiveStream>(json);
        // Assert
        Assert.Equal("TestStream", content!.StreamName);
        Assert.Equal("Test stream", content!.Description);
        Assert.Equal(StreamStatus.Running,content!.StreamStatus);
    }

    [Fact]
    public void Deserialize_NullInput_ReturnsDefault()
    {
        // Act
        var content = _serializer.Deserialize<LiveStream>(null!);
        // Assert
        Assert.Null(content);
    }

    [Fact]
    public void Deserialize_EmptyString_ReturnsDefault()
    {
        // Arrange        
        var json = "";
        // Act
        var content = _serializer.Deserialize<LiveStream>(json);
        // Assert
        Assert.Null(content);
    }

    [Fact]
    public void Deserialize_WhitespaceString_ReturnsDefault()
    {
        // Arrange        
        var json = " ";
        // Act
        var content = _serializer.Deserialize<LiveStream>(json);
        // Assert
        Assert.Null(content);
    }

    [Fact]
    public async Task Deserialize_RoundTrip_PreservesAllValues()
    {
        // Arrange      
        var stream = new LiveStream("TestStream", "Test stream", StreamStatus.Running);
        // Act
        var serializedContent = _serializer.Serialize(stream!);
        var content = await serializedContent.ReadAsStringAsync();
        var deserializedContent = _serializer.Deserialize<LiveStream>(content);
        // Assert
        Assert.Equal("TestStream", deserializedContent!.StreamName);
        Assert.Equal("Test stream", deserializedContent!.Description);
        Assert.Equal(StreamStatus.Running, deserializedContent!.StreamStatus);
    }
}
