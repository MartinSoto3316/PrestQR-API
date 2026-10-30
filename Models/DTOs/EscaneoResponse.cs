using PrestQR_API.Models.Entities;

namespace PrestQR_API.Models.DTOs
{
    /// <summary>
    /// Resultado de escanear un QR: el equipo, su préstamo activo (si lo tiene)
    /// y la única acción que la PWA debe ofrecer.
    /// </summary>
    public class EscaneoResponse
    {
        public const string AccionPrestar = "prestar";
        public const string AccionDevolver = "devolver";

        public Equipo Equipo { get; set; } = null!;
        public Prestamo? PrestamoActivo { get; set; }
        public string AccionPermitida { get; set; } = AccionPrestar;
    }
}
