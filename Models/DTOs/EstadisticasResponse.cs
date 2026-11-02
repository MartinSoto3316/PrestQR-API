namespace PrestQR_API.Models.DTOs
{
    /// <summary>Datos del dashboard del administrador. Fechas calculadas con hora de Hermosillo.</summary>
    public class EstadisticasResponse
    {
        public ResumenEquipos Equipos { get; set; } = new();
        public ResumenPrestamos Prestamos { get; set; } = new();

        /// <summary>Los 5 equipos con más préstamos en todo el historial.</summary>
        public IEnumerable<EquipoMasPrestado> EquiposMasPrestados { get; set; } = [];

        /// <summary>Préstamos registrados en los últimos 6 meses, incluido el actual (los meses sin préstamos van en 0).</summary>
        public IEnumerable<PrestamosPorMes> PrestamosPorMes { get; set; } = [];
    }

    public class ResumenEquipos
    {
        public int Total { get; set; }
        public int Disponibles { get; set; }
        public int Prestados { get; set; }
    }

    public class ResumenPrestamos
    {
        public int Activos { get; set; }
        public int Vencidos { get; set; }
        public int VencenHoy { get; set; }
        public int EsteMes { get; set; }
        public int DevueltosEsteMes { get; set; }
    }

    public class EquipoMasPrestado
    {
        public int EquipoId { get; set; }
        public string CodigoQr { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int Veces { get; set; }
    }

    public class PrestamosPorMes
    {
        /// <summary>Formato yyyy-MM, por ejemplo 2026-11.</summary>
        public string Mes { get; set; } = string.Empty;
        public int Total { get; set; }
    }
}
