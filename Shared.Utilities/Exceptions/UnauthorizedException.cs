namespace Shared.Utilities.Exceptions;

/// <summary>
/// Exception thrown when authentication fails or is required.
/// </summary>
public class UnauthorizedException : BaseException
{
    private const int DefaultStatusCode = 401;
    private const string DefaultErrorCode = "UNAUTHORIZED";

    public UnauthorizedException()
        : base("No autorizado. Se requiere autenticación.", DefaultStatusCode, DefaultErrorCode)
    {
    }

    public UnauthorizedException(string message)
        : base(message, DefaultStatusCode, DefaultErrorCode)
    {
    }

    public UnauthorizedException(string message, Exception innerException)
        : base(message, DefaultStatusCode, DefaultErrorCode, innerException)
    {
    }
}
