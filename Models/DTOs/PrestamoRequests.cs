using System.ComponentModel.DataAnnotations;

namespace PrestQR_API.Models.DTOs
{
    /// <summary>
    /// La fecha de préstamo no se envía: la pone la base de datos con la fecha y hora actuales.
    /// </summary>
    public class CrearPrestamoRequest
    {
        [Required(ErrorMessage = "El equipo es obligatorio.")]
        public int? EquipoId { get; set; }

        [Required(ErrorMessage = "El alumno es obligatorio.")]
        public int? AlumnoId { get; set; }

        /// <summary>Formato yyyy-MM-dd, por ejemplo 2026-11-05.</summary>
        [Required(ErrorMessage = "La fecha límite de devolución es obligatoria.")]
        public DateOnly? FechaLimite { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }
    }

    /// <summary>Filtros de GET /api/prestamos. Todos son opcionales.</summary>
    public class PrestamoFiltro
    {
        /// <summary>activo | completado</summary>
        public string? Estado { get; set; }

        /// <summary>true → solo préstamos activos con la fecha límite vencida.</summary>
        public bool Vencidos { get; set; }

        public int? EquipoId { get; set; }
        public int? AlumnoId { get; set; }
    }
}
