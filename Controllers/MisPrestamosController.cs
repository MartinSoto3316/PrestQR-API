using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrestQR_API.Auth;
using PrestQR_API.Models.DTOs;
using PrestQR_API.Models.Entities;
using PrestQR_API.Services;

namespace PrestQR_API.Controllers
{
    /// <summary>
    /// Préstamos del alumno que inició sesión. El alumno sale del token, nunca de la URL,
    /// así que no puede consultar préstamos de otros.
    /// </summary>
    [ApiController]
    [Authorize(Roles = Roles.Alumno)]
    [Route("api/mis-prestamos")]
    public class MisPrestamosController : ControllerBase
    {
        private readonly IPrestamoService _prestamoService;

        public MisPrestamosController(IPrestamoService prestamoService)
        {
            _prestamoService = prestamoService;
        }

        /// <summary>Filtros opcionales: ?estado=activo|completado&amp;vencidos=true</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prestamo>>> Listar([FromQuery] string? estado, [FromQuery] bool vencidos)
        {
            var alumnoId = User.ObtenerAlumnoId();
            if (alumnoId is null)
            {
                return Forbid();
            }

            return Ok(await _prestamoService.ListarAsync(new PrestamoFiltro
            {
                AlumnoId = alumnoId,
                Estado = estado,
                Vencidos = vencidos
            }));
        }
    }
}
