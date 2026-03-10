using System.Collections.Generic;
using System.Net.Http;
using WowzaV2Client.Requests;

namespace WowzaV2Client.Tests.Unit;

public class RestRequestTests
{
    [Fact]
    public void BuildUri_NoQueryParams_ReturnsPathUnchanged()
    {
        // Arrange
        var request = new RestRequest(
            HttpMethod.Get, 
            "/live_streams/start", 
            new Dictionary<string, string>(), 
            new Dictionary<string, string>()
        );

        // Act
        var uri = request.BuildUri();

        // Assert
        Assert.Equal("/live_streams/start", uri);
    }

    [Fact]
    public void BuildUri_SingleQueryParam_AppendsToPath()
    {
        // Arrange
        var request = new RestRequest(
            HttpMethod.Get, 
            "/live_streams/start", 
            new Dictionary<string, string>(), 
            new Dictionary<string, string> { { "streamId", "123" } }
        );

        // Act
        var uri = request.BuildUri();

        // Assert
        Assert.Equal("/live_streams/start?streamId=123", uri);
    }

    [Fact]
    public void BuildUri_MultipleParams_JoinedWithAmpersand()
    {
        // Arrange
        var request = new RestRequest(
            HttpMethod.Get, 
            "/live_streams/start", 
            new Dictionary<string, string>(), 
            new Dictionary<string, string> { { "streamId", "123" }, { "quality", "high" } }
        );

        // Act
        var uri = request.BuildUri();

        // Assert
        Assert.Equal("/live_streams/start?streamId=123&quality=high", uri);
    }

    [Fact]
    public void BuildUri_SpecialCharactersInKeyAndValue_AreUrlEncoded()
    {
        // Arrange
        var request = new RestRequest(
            HttpMethod.Get, 
            "/live_streams/start", 
            new Dictionary<string, string>(), 
            new Dictionary<string, string> { { "stream name", "My Stream & Co" } }
        );

        // Act
        var uri = request.BuildUri();

        // Assert
        Assert.Equal("/live_streams/start?stream%20name=My%20Stream%20%26%20Co", uri);
    }

    [Fact]
    public void BuildUri_EmptyQueryValue_IsHandledGracefully()
    {
        // Arrange
        var request = new RestRequest(
            HttpMethod.Get, 
            "/live_streams/start", 
            new Dictionary<string, string>(), 
            new Dictionary<string, string> { { "streamId", "" } }
        );

        // Act
        var uri = request.BuildUri();

        // Assert
        Assert.Equal("/live_streams/start?streamId=", uri);
    }
}
