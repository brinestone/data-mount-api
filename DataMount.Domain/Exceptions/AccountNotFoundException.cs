namespace DataMount.Domain.Exceptions;

public enum ErrorCodes
{
    NotFound = 404,
    Unauthorized = 401,
    Conflict = 409,
    Forbidden = 403,
}

public abstract class BaseException(ErrorCodes code, string? message = "") : Exception(message)
{
    public ErrorCodes Code { get; } = code;
}

public class AccountNotFoundException() : BaseException(ErrorCodes.NotFound, "Account not found");

public class UnauthorizedException(string message = "Unauthorized") : BaseException(ErrorCodes.Unauthorized, message);

public class ConflictException(string message) : BaseException(ErrorCodes.Conflict, message);

public class ForbiddenException(string? message = null) : BaseException(ErrorCodes.Forbidden, message);