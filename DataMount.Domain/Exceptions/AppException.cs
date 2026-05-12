namespace DataMount.Domain.Exceptions;

public abstract class AppException(ErrorCodes code, string? message = "") : ApplicationException(message)
{
    public ErrorCodes Code { get; } = code;
}