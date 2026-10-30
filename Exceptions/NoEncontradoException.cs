namespace PrestQR_API.Exceptions
{
    /// <summary>El recurso solicitado no existe. Se responde con 404.</summary>
    public class NoEncontradoException : Exception
    {
        public NoEncontradoException(string mensaje) : base(mensaje) { }
    }
}
