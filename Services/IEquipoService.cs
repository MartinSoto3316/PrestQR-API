using PrestQR_API.Models.DTOs;
using PrestQR_API.Models.Entities;

namespace PrestQR_API.Services
{
    public interface IEquipoService
    {
        Task<IEnumerable<Equipo>> ListarAsync(string? estado);
        Task<Equipo> ObtenerAsync(int id);
        Task<EscaneoResponse> EscanearAsync(string codigoQr);
        Task<Equipo> CrearAsync(CrearEquipoRequest request);
        Task<Equipo> ActualizarAsync(int id, ActualizarEquipoRequest request);
        Task EliminarAsync(int id);
    }
}
