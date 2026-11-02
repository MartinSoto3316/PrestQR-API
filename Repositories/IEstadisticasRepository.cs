using PrestQR_API.Models.DTOs;

namespace PrestQR_API.Repositories
{
    public interface IEstadisticasRepository
    {
        Task<EstadisticasResponse> ObtenerAsync();
    }
}
