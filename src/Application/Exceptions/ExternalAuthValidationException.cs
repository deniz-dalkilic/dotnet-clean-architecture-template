namespace Template.Application.Exceptions;

public sealed class ExternalAuthValidationException(string message) : Exception(message);
