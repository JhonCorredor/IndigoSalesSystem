namespace Shared.Utilities.Exceptions;

/// <summary>
/// Exception thrown when access to a resource is forbidden.
/// </summary>
public class ForbiddenException : BaseException
{
    private const int DefaultStatusCode = 403;
    private const string DefaultErrorCode = "FORBIDDEN";

    public ForbiddenException()
        : base("Acceso denegado. No tiene permisos para realizar esta acción.", DefaultStatusCode, DefaultErrorCode)
    {
    }

    public ForbiddenException(string message)
        : base(message, DefaultStatusCode, DefaultErrorCode)
    {
    }

    public ForbiddenException(string message, Exception innerException)
        : base(message, DefaultStatusCode, DefaultErrorCode, innerException)
    {
    }
}
