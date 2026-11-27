namespace POS_Backend.Logica.Exceptions;

public static class DomainExceptionFilter
{
    public static bool IsUnexpected(Exception ex) =>
        ex is not (BusinessRuleException or NotFoundException or OperationCanceledException);
}