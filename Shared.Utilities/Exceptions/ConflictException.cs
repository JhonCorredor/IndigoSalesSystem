namespace Shared.Utilities.Exceptions;

/// <summary>
/// Exception thrown when there is a conflict with the current state of a resource.
/// </summary>
public class ConflictException : BaseException
{
    private const int DefaultStatusCode = 409;
    private const string DefaultErrorCode = "CONFLICT";

    public ConflictException(string message)
        : base(message, DefaultStatusCode, DefaultErrorCode)
    {
    }

    public ConflictException(string resourceName, object key)
        : base($"El recurso '{resourceName}' con identificador '{key}' ya existe.", DefaultStatusCode, DefaultErrorCode)
    {
    }

    public ConflictException(string message, Exception innerException)
        : base(message, DefaultStatusCode, DefaultErrorCode, innerException)
    {
    }
}
