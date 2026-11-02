using System.ComponentModel.DataAnnotations;

namespace PrestQR_API.Models.DTOs
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no es válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Datos del usuario que inició sesión. La PWA usa el rol para decidir qué pantallas mostrar.
    /// </summary>
    public class UsuarioSesion
    {
        public int UsuarioId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;

        /// <summary>Nombre del alumno; los administradores no tienen nombre propio y se usa su email.</summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>Solo viene cuando el rol es alumno.</summary>
        public int? AlumnoId { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiraEn { get; set; }
        public UsuarioSesion Usuario { get; set; } = null!;
    }
}
