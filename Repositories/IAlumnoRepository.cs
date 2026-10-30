using PrestQR_API.Models.Entities;

namespace PrestQR_API.Repositories
{
    public interface IAlumnoRepository
    {
        Task<IEnumerable<Alumno>> ListarAsync(string? buscar);
        Task<Alumno?> ObtenerPorIdAsync(int id);
        Task<int> CrearAsync(string email, string passwordHash, string matricula, string nombre, string? carrera);
        Task ActualizarAsync(int id, string email, string matricula, string nombre, string? carrera);
        Task<bool> CambiarActivoAsync(int id, bool activo);
    }
}
