using PrestQR_API.Models.DTOs;
using PrestQR_API.Models.Entities;

namespace PrestQR_API.Repositories
{
    public interface IPrestamoRepository
    {
        Task<IEnumerable<Prestamo>> ListarAsync(PrestamoFiltro filtro);
        Task<Prestamo?> ObtenerPorIdAsync(int id);
        Task<Prestamo?> ObtenerActivoPorEquipoAsync(int equipoId);
        Task<int> RegistrarAsync(int equipoId, int alumnoId, int adminId, DateOnly fechaLimite, string? observaciones);
        Task RegistrarDevolucionAsync(int prestamoId);
    }
}
