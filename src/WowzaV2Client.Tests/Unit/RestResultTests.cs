using System;
using System.Net;
using WowzaV2Client.Responses;

namespace WowzaV2Client.Tests.Unit;


record UserDto(int Id, string Name);

public class RestResultTests
{
    [Fact]
    public void Success_SetsIsSuccessTrue()
    {
        // Arrange
        var value = new UserDto(1, "Joyce");

        // Act
        var result = RestResult<UserDto>.Success(value, HttpStatusCode.OK);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
    }

    [Fact]
    public void Success_ValueReturnsTheObject()
    {
        // Arrange
        var value = new UserDto(1, "Joyce");

        // Act
        var result = RestResult<UserDto>.Success(value, HttpStatusCode.OK);

        // Assert
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public void Success_StatusCodeIsPreserved()
    {
        // Arrange
        var value = new UserDto(1, "Joyce");

        // Act
        var result = RestResult<UserDto>.Success(value, HttpStatusCode.OK);

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public void Failure_SetsIsFailureTrue()
    {
        // Arrange
        var error = new RestError("There was an error");

        // Act
        var result = RestResult<UserDto>.Failure(error, HttpStatusCode.InternalServerError);

        // Assert
        Assert.True(result.IsFailure);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Failure_ErrorReturnsTheError()
    {
        // Arrange
        var error = new RestError("There was an error");

        // Act
        var result = RestResult<UserDto>.Failure(error, HttpStatusCode.InternalServerError);

        // Assert
        Assert.Equal("There was an error", result.Error!.Message);
        
    }

    [Fact]
    public void Value_OnFailure_ThrowsInvalidOperationException()
    {
        // Arrange
        var error = new RestError("There was an error");

        // Act
        var result = RestResult<UserDto>.Failure(error, HttpStatusCode.InternalServerError);

        // Assert
        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void Error_OnSuccess_ThrowsInvalidOperationException()
    {
        // Arrange
        var value = new UserDto(1, "Joyce");

        // Act
        var result = RestResult<UserDto>.Success(value, HttpStatusCode.OK);

        // Assert
        Assert.Throws<InvalidOperationException>(() => result.Error);
    }

    [Fact]
    public void Map_OnSuccess_TransformsValue()
    {
        // Arrange
        var value = new UserDto(1, "Joyce");

        // Act
        var result = RestResult<UserDto>.Success(value, HttpStatusCode.OK);
        var nameResult = result.Map(user => user.Name);

        // Assert
        Assert.Equal("Joyce", nameResult.Value);
    }

    [Fact]
    public void Map_OnFailure_PreservesError()
    {
        // Arrange
        var error = new RestError("There was an error");

        // Act
        var result = RestResult<UserDto>.Failure(error, HttpStatusCode.InternalServerError);
        var errorResult = result.Map(user => user.Name);

        // Assert
        Assert.Equal("There was an error", errorResult.Error!.Message);
    }

    [Fact]
    public void Deconstruct_OnSuccess_YieldsCorrectValues()
    {
        // Arrange
        var value = new UserDto(1, "Joyce");

        // Act
        var result = RestResult<UserDto>.Success(value, HttpStatusCode.OK);
        var (ok, user, error) = result;

        // Assert
        Assert.True(ok);
        Assert.Equal(value, user);
        Assert.Null(error);
    }

    [Fact]
    public void Deconstruct_OnFailure_YieldsCorrectValues()
    {
        // Arrange
        var restError = new RestError("There was an error");

        // Act
        var result = RestResult<UserDto>.Failure(restError, HttpStatusCode.InternalServerError);
        var (ok, user, error) = result;

        // Assert
        Assert.False(ok);
        Assert.Null(user);
        Assert.NotNull(error);
    }

    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public void StatusCode_IsPreserved(HttpStatusCode statusCode)
    {
        // Arrange
        var value = new UserDto(1, "Joyce");
        var error = new RestError("There was an error");

        // Act
        var successResult = RestResult<UserDto>.Success(value, statusCode);
        var errorResult = RestResult<UserDto>.Failure(error, statusCode);

        // Assert
        Assert.Equal(statusCode, successResult.StatusCode);
        Assert.Equal(statusCode, errorResult.StatusCode);
    }
}
