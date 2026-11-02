namespace PrestQR_API.Exceptions
{
    /// <summary>Credenciales inválidas o cuenta desactivada. Se responde con 401.</summary>
    public class NoAutorizadoException : Exception
    {
        public NoAutorizadoException(string mensaje) : base(mensaje) { }
    }
}
