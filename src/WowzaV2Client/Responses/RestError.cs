using System;

namespace WowzaV2Client.Responses;

public record RestError(
    string Message, 
    string? Detail = null,
    Exception? Exception = null
);
