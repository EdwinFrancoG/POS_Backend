namespace POS_Backend.Logica.Exceptions;

public class BusinessRuleException : Exception
{
    public IReadOnlyList<string> Errors { get; }

    public BusinessRuleException(IEnumerable<string> errors)
        : base(string.Join(" ", errors))
    {
        Errors = errors.ToList();
    }

    public BusinessRuleException(string error) : this(new[] { error }) { }
}

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

public class DomainException : Exception
{
    public DomainException(string message, Exception innerException)
        : base(message, innerException) { }
}