using Microsoft.AspNetCore.Mvc;
using PrestQR_API.Models.DTOs;
using PrestQR_API.Models.Entities;
using PrestQR_API.Services;

namespace PrestQR_API.Controllers
{
    [ApiController]
    [Route("api/alumnos")]
    public class AlumnosController : ControllerBase
    {
        private readonly IAlumnoService _alumnoService;

        public AlumnosController(IAlumnoService alumnoService)
        {
            _alumnoService = alumnoService;
        }

        /// <summary>Lista los alumnos. Búsqueda opcional por nombre o matrícula: ?buscar=soto</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Alumno>>> Listar([FromQuery] string? buscar)
        {
            return Ok(await _alumnoService.ListarAsync(buscar));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Alumno>> ObtenerPorId(int id)
        {
            return Ok(await _alumnoService.ObtenerAsync(id));
        }

        /// <summary>Historial de préstamos del alumno (activos y completados).</summary>
        [HttpGet("{id:int}/prestamos")]
        public async Task<ActionResult<IEnumerable<Prestamo>>> ListarPrestamos(int id)
        {
            return Ok(await _alumnoService.ListarPrestamosAsync(id));
        }

        [HttpPost]
        public async Task<ActionResult<Alumno>> Crear(CrearAlumnoRequest request)
        {
            var alumno = await _alumnoService.CrearAsync(request);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = alumno.Id }, alumno);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Alumno>> Actualizar(int id, ActualizarAlumnoRequest request)
        {
            return Ok(await _alumnoService.ActualizarAsync(id, request));
        }

        /// <summary>Activa o desactiva la cuenta. Los alumnos no se borran porque tienen historial.</summary>
        [HttpPatch("{id:int}/activo")]
        public async Task<ActionResult<Alumno>> CambiarActivo(int id, CambiarActivoRequest request)
        {
            return Ok(await _alumnoService.CambiarActivoAsync(id, request.Activo!.Value));
        }
    }
}
