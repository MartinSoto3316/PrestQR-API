using PrestQR_API.Models.DTOs;

namespace PrestQR_API.Services
{
    public interface IEstadisticasService
    {
        Task<EstadisticasResponse> ObtenerAsync();
    }
}
