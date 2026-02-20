namespace Shared.Utilities.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found.
/// </summary>
public class NotFoundException : BaseException
{
    private const int DefaultStatusCode = 404;
    private const string DefaultErrorCode = "RESOURCE_NOT_FOUND";

    public NotFoundException(string message)
        : base(message, DefaultStatusCode, DefaultErrorCode)
    {
    }

    public NotFoundException(string resourceName, object key)
        : base($"El recurso '{resourceName}' con identificador '{key}' no fue encontrado.", DefaultStatusCode, DefaultErrorCode)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, DefaultStatusCode, DefaultErrorCode, innerException)
    {
    }
}
