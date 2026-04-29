namespace DataMount.Domain.Exceptions;

public enum ErrorCodes
{
    NotFound = 404,
    Unauthorized = 401
}

public abstract class BaseException(ErrorCodes code, string? message = null) : Exception(message);

public class AccountNotFoundException() : BaseException(ErrorCodes.NotFound, "Account not found");

public class UnauthorizedException(string message = "Unauthorized") : BaseException(ErrorCodes.Unauthorized, message);