namespace DataMount.Domain.Exceptions;

public class ForbiddenException(string? message = null) : AppException(ErrorCodes.Forbidden, message);