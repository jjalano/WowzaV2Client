using System;
using System.Net.Http;

namespace WowzaV2Client.Tests.Helpers;

public static class TestHttpClientFactory
{
    public static HttpClient CreateHttpClient(FakeHttpMessageHandler handler)
    {
        return new HttpClient(handler)
        {
            BaseAddress = new Uri("https://test.example.com")
        };
    }
}
