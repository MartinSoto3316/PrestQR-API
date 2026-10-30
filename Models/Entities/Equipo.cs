namespace PrestQR_API.Models.Entities
{
    public class Equipo
    {
        public int Id { get; set; }
        public string CodigoQr { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Modelo { get; set; }
        public string Estado { get; set; } = EstadosEquipo.Disponible;
        public DateTime CreadoEn { get; set; }
    }

    public static class EstadosEquipo
    {
        public const string Disponible = "disponible";
        public const string Prestado = "prestado";

        public static readonly string[] Todos = [Disponible, Prestado];
    }
}
