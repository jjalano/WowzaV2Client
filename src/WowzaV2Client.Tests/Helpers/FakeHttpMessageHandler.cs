using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WowzaV2Client.Tests.Helpers;

public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly Queue<Func<HttpRequestMessage, HttpResponseMessage>> _responseQueue = new();
    public HttpRequestMessage? LastRequest { get; private set; }
    public int CallCount { get; private set; }

    public FakeHttpMessageHandler(IEnumerable<HttpResponseMessage> responses)
    {
        foreach (var response in responses)
            _responseQueue.Enqueue(_ => response);
    }

    private FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> factory)
    {
        _responseQueue.Enqueue(factory);
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequest = request;
        CallCount++;

        if (_responseQueue.Count == 0)
            throw new InvalidOperationException("FakeHttpMessageHandler has no more responses queued");

        var factory = _responseQueue.Dequeue();
        return Task.FromResult(factory(request));

    }

    // <summary>
    /// Returns a single response with the given status code and optional string content.
    /// </summary>
    public static FakeHttpMessageHandler Returns(
        HttpStatusCode statusCode,
        string content = "") =>
        new(_ => new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(content)
        });

    /// <summary>
    /// Throws the given exception when SendAsync is called, simulating a network failure.
    /// </summary>
    public static FakeHttpMessageHandler Throws(Exception exception) =>
        new(_ => throw exception);
}
