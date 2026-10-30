namespace PrestQR_API.Models.Entities
{
    /// <summary>Alumno junto con los datos de su cuenta (email y si está activa). Nunca incluye la contraseña.</summary>
    public class Alumno
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Matricula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Carrera { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }
}
