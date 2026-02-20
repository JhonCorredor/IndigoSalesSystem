using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Utilities.Exceptions;
using Shared.Utilities.Responses;

namespace Shared.Utilities.Middleware;

/// <summary>
/// Global exception handling middleware.
/// Catches all exceptions and returns standardized API responses.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, response) = exception switch
        {
            ValidationException validationEx => (
                validationEx.StatusCode,
                ApiResponse.Fail(validationEx.Message, validationEx.Errors, validationEx.ErrorCode)
            ),
            BaseException baseEx => (
                baseEx.StatusCode,
                ApiResponse.Fail(baseEx.Message, baseEx.ErrorCode)
            ),
            ArgumentException argEx => (
                StatusCodes.Status400BadRequest,
                ApiResponse.Fail(argEx.Message, "INVALID_ARGUMENT")
            ),
            InvalidOperationException invalidOpEx => (
                StatusCodes.Status422UnprocessableEntity,
                ApiResponse.Fail(invalidOpEx.Message, "INVALID_OPERATION")
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                ApiResponse.Fail("Ha ocurrido un error interno en el servidor.", "INTERNAL_SERVER_ERROR")
            )
        };

        LogException(exception, statusCode);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(response, JsonOptions);
        await context.Response.WriteAsync(json);
    }

    private void LogException(Exception exception, int statusCode)
    {
        if (statusCode >= 500)
        {
            _logger.LogError(exception, "Error interno del servidor: {Message}", exception.Message);
        }
        else if (statusCode >= 400)
        {
            _logger.LogWarning("Error de cliente ({StatusCode}): {Message}", statusCode, exception.Message);
        }
    }
}
