namespace PrestQR_API.Exceptions
{
    /// <summary>La operación rompe una regla de negocio. Se responde con 409.</summary>
    public class ReglaNegocioException : Exception
    {
        public ReglaNegocioException(string mensaje) : base(mensaje) { }
    }
}
