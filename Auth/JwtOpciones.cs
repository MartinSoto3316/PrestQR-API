namespace PrestQR_API.Auth
{
    /// <summary>
    /// Sección "Jwt" de appsettings. La clave NO va en appsettings.json:
    /// en desarrollo va en appsettings.Development.json y en el hosting en la variable de entorno Jwt__Clave.
    /// </summary>
    public class JwtOpciones
    {
        public const string Seccion = "Jwt";

        public string Emisor { get; set; } = string.Empty;
        public string Audiencia { get; set; } = string.Empty;

        /// <summary>Al menos 32 caracteres (HMAC-SHA256 necesita 256 bits).</summary>
        public string Clave { get; set; } = string.Empty;

        public int MinutosExpiracion { get; set; } = 480;
    }
}
