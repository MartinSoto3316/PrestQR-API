using PrestQR_API.Models.Entities;

namespace PrestQR_API.Repositories
{
    public interface IEquipoRepository
    {
        Task<IEnumerable<Equipo>> ListarAsync(string? estado);
        Task<Equipo?> ObtenerPorIdAsync(int id);
        Task<Equipo?> ObtenerPorCodigoQrAsync(string codigoQr);
        Task<int> CrearAsync(string codigoQr, string nombre, string? modelo);
        Task<bool> ActualizarAsync(int id, string nombre, string? modelo);
        Task<bool> TienePrestamosAsync(int id);
        Task<bool> EliminarAsync(int id);
    }
}
