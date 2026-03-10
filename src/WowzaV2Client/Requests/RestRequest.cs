using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace WowzaV2Client.Requests;

public record RestRequest (
    HttpMethod HttpMethod, 
    string Path, 
    Dictionary<string, string> Headers,
    Dictionary<string, string> QueryParameters,
    HttpContent? Body = null)
{
    public string BuildUri()
    {
        if (QueryParameters.Count == 0)
        {
            return Path;
        }

        var queryString = string.Join("&", QueryParameters.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
        return $"{Path}?{queryString}";
    }
}


