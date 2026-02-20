namespace Shared.Utilities.Exceptions;

/// <summary>
/// Exception thrown when validation fails.
/// </summary>
public class ValidationException : BaseException
{
    private const int DefaultStatusCode = 400;
    private const string DefaultErrorCode = "VALIDATION_ERROR";

    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(string message)
        : base(message, DefaultStatusCode, DefaultErrorCode)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("Se han producido uno o más errores de validación.", DefaultStatusCode, DefaultErrorCode)
    {
        Errors = errors;
    }

    public ValidationException(string propertyName, string errorMessage)
        : base(errorMessage, DefaultStatusCode, DefaultErrorCode)
    {
        Errors = new Dictionary<string, string[]>
        {
            { propertyName, [errorMessage] }
        };
    }
}
