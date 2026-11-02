using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrestQR_API.Auth;
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
    [Authorize(Roles = Roles.Admin)]
    [Route("api/prestamos")]
    public class PrestamosController : ControllerBase
    {
        private readonly IPrestamoService _prestamoService;

        public PrestamosController(IPrestamoService prestamoService)
        {
            _prestamoService = prestamoService;
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
            // El préstamo queda a nombre del administrador que inició sesión.
            var prestamo = await _prestamoService.RegistrarAsync(request, User.ObtenerUsuarioId());
            return CreatedAtAction(nameof(ObtenerPorId), new { id = prestamo.Id }, prestamo);
        }

        /// <summary>Registra la devolución. Préstamo: activo → completado. Equipo: prestado → disponible.</summary>
        [HttpPost("{id:int}/devolucion")]
        public async Task<ActionResult<Prestamo>> RegistrarDevolucion(int id)
        {
            return Ok(await _prestamoService.RegistrarDevolucionAsync(id));
        }
    }
}
