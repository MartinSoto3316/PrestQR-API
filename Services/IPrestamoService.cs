using PrestQR_API.Models.DTOs;
using PrestQR_API.Models.Entities;

namespace PrestQR_API.Services
{
    public interface IPrestamoService
    {
        Task<IEnumerable<Prestamo>> ListarAsync(PrestamoFiltro filtro);
        Task<Prestamo> ObtenerAsync(int id);
        Task<Prestamo> RegistrarAsync(CrearPrestamoRequest request, int adminId);
        Task<Prestamo> RegistrarDevolucionAsync(int id);
    }
}
