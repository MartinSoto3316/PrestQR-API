using PrestQR_API.Models.DTOs;
using PrestQR_API.Models.Entities;

namespace PrestQR_API.Services
{
    public interface IAlumnoService
    {
        Task<IEnumerable<Alumno>> ListarAsync(string? buscar);
        Task<Alumno> ObtenerAsync(int id);
        Task<IEnumerable<Prestamo>> ListarPrestamosAsync(int id);
        Task<Alumno> CrearAsync(CrearAlumnoRequest request);
        Task<Alumno> ActualizarAsync(int id, ActualizarAlumnoRequest request);
        Task<Alumno> CambiarActivoAsync(int id, bool activo);
    }
}
