namespace PrestQR_API.Models.Entities
{
    /// <summary>
    /// Préstamo junto con los datos del equipo y del alumno que se muestran en pantalla.
    /// </summary>
    public class Prestamo
    {
        public int Id { get; set; }

        public int EquipoId { get; set; }
        public string EquipoCodigoQr { get; set; } = string.Empty;
        public string EquipoNombre { get; set; } = string.Empty;

        public int AlumnoId { get; set; }
        public string AlumnoMatricula { get; set; } = string.Empty;
        public string AlumnoNombre { get; set; } = string.Empty;

        public int AdminId { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public DateOnly FechaLimite { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public string Estado { get; set; } = EstadosPrestamo.Activo;
        public string? Observaciones { get; set; }

        /// <summary>Activo y con la fecha límite ya pasada (hora de Hermosillo). Se calcula al consultar.</summary>
        public bool Vencido { get; set; }
    }

    public static class EstadosPrestamo
    {
        public const string Activo = "activo";
        public const string Completado = "completado";

        public static readonly string[] Todos = [Activo, Completado];
    }
}
