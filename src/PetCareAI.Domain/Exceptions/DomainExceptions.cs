namespace PetCareAI.Domain.Exceptions;

public class DomainException(string message) : Exception(message);
public class NotFoundException(string message) : Exception(message);
public class ConflictException(string message) : Exception(message);
public class UnauthorizedAppException(string message) : Exception(message);