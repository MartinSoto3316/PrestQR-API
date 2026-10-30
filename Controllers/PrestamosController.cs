using Microsoft.AspNetCore.Mvc;
using PrestQR_API.Models.DTOs;
using PrestQR_API.Models.Entities;
using PrestQR_API.Services;

namespace PrestQR_API.Controllers
{
    /// <summary>
    /// Los préstamos no tienen PUT ni DELETE: son el historial del sistema.
    /// La devolución es una acción de negocio con su propia ruta.
    /// </summary>
    [ApiController]
    [Route("api/prestamos")]
    public class PrestamosController : ControllerBase
    {
        private readonly IPrestamoService _prestamoService;
        private readonly IConfiguration _configuration;

        public PrestamosController(IPrestamoService prestamoService, IConfiguration configuration)
        {
            _prestamoService = prestamoService;
            _configuration = configuration;
        }

        /// <summary>
        /// Lista préstamos. Filtros opcionales: ?estado=activo|completado&amp;vencidos=true&amp;equipoId=1&amp;alumnoId=1
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prestamo>>> Listar([FromQuery] PrestamoFiltro filtro)
        {
            return Ok(await _prestamoService.ListarAsync(filtro));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Prestamo>> ObtenerPorId(int id)
        {
            return Ok(await _prestamoService.ObtenerAsync(id));
        }

        /// <summary>Registra un préstamo. Equipo: disponible → prestado.</summary>
        [HttpPost]
        public async Task<ActionResult<Prestamo>> Registrar(CrearPrestamoRequest request)
        {
            var prestamo = await _prestamoService.RegistrarAsync(request, ObtenerAdminId());
            return CreatedAtAction(nameof(ObtenerPorId), new { id = prestamo.Id }, prestamo);
        }

        /// <summary>Registra la devolución. Préstamo: activo → completado. Equipo: prestado → disponible.</summary>
        [HttpPost("{id:int}/devolucion")]
        public async Task<ActionResult<Prestamo>> RegistrarDevolucion(int id)
        {
            return Ok(await _prestamoService.RegistrarDevolucionAsync(id));
        }

        // Temporal hasta tener login: cuando agreguemos JWT, el id saldrá del token del administrador.
        private int ObtenerAdminId()
        {
            return _configuration.GetValue<int?>("PrestQR:AdminIdTemporal")
                ?? throw new InvalidOperationException("Falta configurar PrestQR:AdminIdTemporal.");
        }
    }
}
