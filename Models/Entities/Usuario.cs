namespace PrestQR_API.Models.Entities
{
    /// <summary>
    /// Cuenta para iniciar sesión. Incluye el hash de la contraseña,
    /// así que solo se usa dentro de la API y nunca se devuelve al cliente.
    /// </summary>
    public class Usuario
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public bool Activo { get; set; }

        /// <summary>Solo tienen valor si el usuario es alumno.</summary>
        public int? AlumnoId { get; set; }
        public string? AlumnoNombre { get; set; }
    }

    public static class Roles
    {
        public const string Admin = "admin";
        public const string Alumno = "alumno";
    }
}
