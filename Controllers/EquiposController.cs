using Microsoft.AspNetCore.Mvc;
using PrestQR_API.Models.DTOs;
using PrestQR_API.Models.Entities;
using PrestQR_API.Services;

namespace PrestQR_API.Controllers
{
    [ApiController]
    [Route("api/equipos")]
    public class EquiposController : ControllerBase
    {
        private readonly IEquipoService _equipoService;

        public EquiposController(IEquipoService equipoService)
        {
            _equipoService = equipoService;
        }

        /// <summary>Lista los equipos. Filtro opcional: ?estado=disponible|prestado</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Equipo>>> Listar([FromQuery] string? estado)
        {
            return Ok(await _equipoService.ListarAsync(estado));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Equipo>> ObtenerPorId(int id)
        {
            return Ok(await _equipoService.ObtenerAsync(id));
        }

        /// <summary>Escaneo de QR: devuelve el equipo, su préstamo activo y la acción permitida.</summary>
        [HttpGet("qr/{codigoQr}")]
        public async Task<ActionResult<EscaneoResponse>> Escanear(string codigoQr)
        {
            return Ok(await _equipoService.EscanearAsync(codigoQr));
        }

        [HttpPost]
        public async Task<ActionResult<Equipo>> Crear(CrearEquipoRequest request)
        {
            var equipo = await _equipoService.CrearAsync(request);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = equipo.Id }, equipo);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Equipo>> Actualizar(int id, ActualizarEquipoRequest request)
        {
            return Ok(await _equipoService.ActualizarAsync(id, request));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            await _equipoService.EliminarAsync(id);
            return NoContent();
        }
    }
}
