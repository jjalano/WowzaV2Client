using System;
using System.Net;
namespace WowzaV2Client.Responses;

public class RestResult<T>
{
    private T? _value;
    private RestError? _error;

    public T? Value 
    { 
        get {
            if (IsFailure)
                throw new InvalidOperationException();
            
            return _value;
        } 
    }
    
    public RestError? Error { 
        get { 
            if (IsSuccess)
                throw new InvalidOperationException();
            
            return _error;
        } 
    }
    
    public HttpStatusCode StatusCode { get; private set; }
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;

    private RestResult(T value, HttpStatusCode httpStatusCode)
    {
        _value = value;
        StatusCode = httpStatusCode;

        IsSuccess = true;
    }

    private RestResult(RestError error, HttpStatusCode httpStatusCode)
    {
        _error = error;
        StatusCode = httpStatusCode;

        IsSuccess = false;
    }

    public RestResult<TResult> Map<TResult>(Func<T, TResult> mapper)
    {
        if (IsSuccess)
            return RestResult<TResult>.Success(mapper.Invoke(_value!), StatusCode);

        return RestResult<TResult>.Failure(_error!, StatusCode);
    }

    public void Deconstruct(out bool isSuccess, out T? value, out RestError? error)
    {
        isSuccess = IsSuccess;
        value = _value;
        error = _error;
    }

    public static RestResult<T> Success(T value, HttpStatusCode httpStatusCode)
    {
        return new RestResult<T>(value, httpStatusCode);
    }

    public static RestResult<T> Failure(RestError error, HttpStatusCode httpStatusCode)
    {
        return new RestResult<T>(error, httpStatusCode);
    }

    

}


