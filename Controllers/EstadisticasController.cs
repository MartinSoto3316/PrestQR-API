using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrestQR_API.Models.DTOs;
using PrestQR_API.Models.Entities;
using PrestQR_API.Services;

namespace PrestQR_API.Controllers
{
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    [Route("api/estadisticas")]
    public class EstadisticasController : ControllerBase
    {
        private readonly IEstadisticasService _estadisticasService;

        public EstadisticasController(IEstadisticasService estadisticasService)
        {
            _estadisticasService = estadisticasService;
        }

        /// <summary>Resumen para el dashboard: equipos, préstamos, equipos más prestados y préstamos por mes.</summary>
        [HttpGet]
        public async Task<ActionResult<EstadisticasResponse>> Obtener()
        {
            return Ok(await _estadisticasService.ObtenerAsync());
        }
    }
}
