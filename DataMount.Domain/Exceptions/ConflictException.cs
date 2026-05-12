namespace DataMount.Domain.Exceptions;

public class ConflictException(string message) : AppException(ErrorCodes.Conflict, message);