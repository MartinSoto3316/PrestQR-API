using System.ComponentModel.DataAnnotations;

namespace PrestQR_API.Models.DTOs
{
    public class CrearEquipoRequest
    {
        /// <summary>Identificador impreso en el QR, por ejemplo LAP-001.</summary>
        [Required(ErrorMessage = "El código QR es obligatorio.")]
        [RegularExpression(@"^[A-Za-z0-9-]{3,20}$", ErrorMessage = "El código QR debe tener de 3 a 20 caracteres: letras, números o guiones.")]
        public string CodigoQr { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Modelo { get; set; }
    }

    /// <summary>
    /// El código QR no se edita porque ya está impreso en el equipo,
    /// y el estado solo cambia mediante un préstamo o una devolución.
    /// </summary>
    public class ActualizarEquipoRequest
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Modelo { get; set; }
    }
}
