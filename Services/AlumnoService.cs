using PrestQR_API.Exceptions;
using PrestQR_API.Models.DTOs;
using PrestQR_API.Models.Entities;
using PrestQR_API.Repositories;

namespace PrestQR_API.Services
{
    public class AlumnoService : IAlumnoService
    {
        private readonly IAlumnoRepository _alumnos;
        private readonly IPrestamoRepository _prestamos;

        public AlumnoService(IAlumnoRepository alumnos, IPrestamoRepository prestamos)
        {
            _alumnos = alumnos;
            _prestamos = prestamos;
        }

        public Task<IEnumerable<Alumno>> ListarAsync(string? buscar)
        {
            return _alumnos.ListarAsync(buscar);
        }

        public async Task<Alumno> ObtenerAsync(int id)
        {
            return await _alumnos.ObtenerPorIdAsync(id)
                ?? throw new NoEncontradoException($"El alumno {id} no existe.");
        }

        public async Task<IEnumerable<Prestamo>> ListarPrestamosAsync(int id)
        {
            await ObtenerAsync(id); // 404 si el alumno no existe
            return await _prestamos.ListarAsync(new PrestamoFiltro { AlumnoId = id });
        }

        public async Task<Alumno> CrearAsync(CrearAlumnoRequest request)
        {
            // La contraseña nunca se guarda en texto plano.
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var id = await _alumnos.CrearAsync(
                NormalizarEmail(request.Email),
                passwordHash,
                request.Matricula.Trim(),
                request.Nombre.Trim(),
                request.Carrera?.Trim());

            return await ObtenerAsync(id);
        }

        public async Task<Alumno> ActualizarAsync(int id, ActualizarAlumnoRequest request)
        {
            // fn_actualizar_alumno lanza PQ404 si el alumno no existe.
            await _alumnos.ActualizarAsync(
                id,
                NormalizarEmail(request.Email),
                request.Matricula.Trim(),
                request.Nombre.Trim(),
                request.Carrera?.Trim());

            return await ObtenerAsync(id);
        }

        public async Task<Alumno> CambiarActivoAsync(int id, bool activo)
        {
            if (!await _alumnos.CambiarActivoAsync(id, activo))
            {
                throw new NoEncontradoException($"El alumno {id} no existe.");
            }

            return await ObtenerAsync(id);
        }

        private static string NormalizarEmail(string email) => email.Trim().ToLowerInvariant();
    }
}
