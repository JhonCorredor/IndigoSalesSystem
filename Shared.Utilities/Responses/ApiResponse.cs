using System.Text.Json.Serialization;

namespace Shared.Utilities.Responses;

/// <summary>
/// Standardized API response wrapper.
/// </summary>
/// <typeparam name="T">Type of the response data.</typeparam>
public class ApiResponse<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorCode { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, string[]>? Errors { get; init; }

    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Creates a successful response with data.
    /// </summary>
    public static ApiResponse<T> Ok(T data, string message = "Operación exitosa")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    /// <summary>
    /// Creates a successful response without data.
    /// </summary>
    public static ApiResponse<T> Ok(string message = "Operación exitosa")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message
        };
    }

    /// <summary>
    /// Creates a failure response with error details.
    /// </summary>
    public static ApiResponse<T> Fail(string message, string? errorCode = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode
        };
    }

    /// <summary>
    /// Creates a failure response with validation errors.
    /// </summary>
    public static ApiResponse<T> Fail(string message, IDictionary<string, string[]> errors, string? errorCode = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors,
            ErrorCode = errorCode
        };
    }
}

/// <summary>
/// Non-generic API response for operations without data.
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    public new static ApiResponse Ok(string message = "Operación exitosa")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message
        };
    }

    public new static ApiResponse Fail(string message, string? errorCode = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode
        };
    }

    public new static ApiResponse Fail(string message, IDictionary<string, string[]> errors, string? errorCode = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = errors,
            ErrorCode = errorCode
        };
    }
}
