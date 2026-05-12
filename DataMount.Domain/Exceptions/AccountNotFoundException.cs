namespace DataMount.Domain.Exceptions;

public class AccountNotFoundException() : AppException(ErrorCodes.NotFound, "Account not found");