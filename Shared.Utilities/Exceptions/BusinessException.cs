namespace Shared.Utilities.Exceptions;

/// <summary>
/// Exception thrown when a business rule is violated.
/// </summary>
public class BusinessException : BaseException
{
    private const int DefaultStatusCode = 422;
    private const string DefaultErrorCode = "BUSINESS_RULE_VIOLATION";

    public BusinessException(string message)
        : base(message, DefaultStatusCode, DefaultErrorCode)
    {
    }

    public BusinessException(string message, string errorCode)
        : base(message, DefaultStatusCode, errorCode)
    {
    }

    public BusinessException(string message, Exception innerException)
        : base(message, DefaultStatusCode, DefaultErrorCode, innerException)
    {
    }
}
