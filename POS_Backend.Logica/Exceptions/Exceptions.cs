namespace POS_Backend.Logica.Exceptions;

public class ReglaNegocioException : Exception
{
    public IReadOnlyList<string> Errores { get; }

    public ReglaNegocioException(IEnumerable<string> errores)
        : base(string.Join(" ", errores))
    {
        Errores = errores.ToList();
    }

    public ReglaNegocioException(string error) : this(new[] { error }) { }
}

public class NoEncontradoException : Exception
{
    public NoEncontradoException(string mensaje) : base(mensaje) { }
}