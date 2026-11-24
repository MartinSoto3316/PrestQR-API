using PrestQR_API.Models.DTOs;
using PrestQR_API.Repositories;

namespace PrestQR_API.Services
{
    /// <summary>Por ahora solo consulta; se mantiene la capa para seguir el mismo patrón que el resto de la API.</summary>
    public class EstadisticasService : IEstadisticasService
    {
        private readonly IEstadisticasRepository _estadisticas;

        public EstadisticasService(IEstadisticasRepository estadisticas)
        {
            _estadisticas = estadisticas;
        }

        public Task<EstadisticasResponse> ObtenerAsync()
        {
            return _estadisticas.ObtenerAsync();
        }
    }
}
