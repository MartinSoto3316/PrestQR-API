namespace PrestQR_API.Exceptions
{
    /// <summary>Un dato de la petición no es válido. Se responde con 400.</summary>
    public class ValidacionException : Exception
    {
        public ValidacionException(string mensaje) : base(mensaje) { }
    }
}
